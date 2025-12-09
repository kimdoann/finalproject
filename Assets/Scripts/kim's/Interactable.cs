using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    public enum FacingAxis
    {
        Right,
        Up
    }

    public enum InteractionType
    {
        OpenPanel,
        CraftingSlotsOnly
    }

    [Header("Interaction Mode")]
    public InteractionType interactionType = InteractionType.OpenPanel;

    [Header("Interaction Settings")]
    public string playerTag = "Player";
    public float interactionDistance = 3f;
    [Range(0f, 180f)]
    public float facingAngleThreshold = 60f;

    [Header("2D Facing Axis")]
    public FacingAxis facingAxis = FacingAxis.Right;

    [Header("UI Settings")]
    public GameObject interactUI;
    public GameObject panelToOpen;
    public bool pauseOnOpen = false;
    public bool manageCursorOnOpen = true;

    [Header("Crafting References (for CraftingSlotsOnly)")]
    public SlotEnterActivator craftingActivator;

    [Header("Events")]
    public UnityEvent OnInteract;
    public UnityEvent OnEnterRange;
    public UnityEvent OnExitRange;

    private bool isPlayerInRange = false;
    private bool isPlayerFacing = false;
    private GameObject playerObject;
    private Collider2D triggerCollider2D;
    private bool panelIsOpen = false;

    void Start()
    {
        triggerCollider2D = GetComponent<Collider2D>();
        if (triggerCollider2D == null)
        {
            triggerCollider2D = gameObject.AddComponent<CircleCollider2D>();
            Debug.Log($"[Interactable2D] {gameObject.name}: Added CircleCollider2D automatically");
        }

        triggerCollider2D.isTrigger = true;
        Debug.Log($"[Interactable2D] {gameObject.name}: Collider2D set as trigger. Player Tag: '{playerTag}'");

        if (triggerCollider2D is CircleCollider2D circle)
        {
            circle.radius = interactionDistance;
        }

        if (interactUI != null)
        {
            interactUI.SetActive(false);
            Debug.Log($"[Interactable2D] {gameObject.name}: UI initialized and hidden");
        }
        else
        {
            Debug.LogWarning($"[Interactable2D] {gameObject.name}: No Interact UI assigned!");
        }

        if (panelToOpen != null)
        {
            panelToOpen.SetActive(false);
            Debug.Log($"[Interactable2D] {gameObject.name}: Panel initialized and hidden");
        }
        else if (interactionType == InteractionType.OpenPanel)
        {
            Debug.LogWarning($"[Interactable2D] {gameObject.name}: No Panel assigned to open (OpenPanel mode)!");
        }

        if (interactionType == InteractionType.CraftingSlotsOnly && craftingActivator == null)
        {
            Debug.LogWarning($"[Interactable2D] {gameObject.name}: CraftingSlotsOnly selected but no SlotEnterActivator assigned.");
        }
    }

    void Update()
    {
        if (panelIsOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }

        if (isPlayerInRange && playerObject != null && !panelIsOpen)
        {
            CheckPlayerFacing2D();

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (isPlayerFacing)
                {
                    Interact();
                }
                else
                {
                    Debug.Log($"[Interactable2D] {gameObject.name}: Player pressed E but is not facing the object. Turn towards it to interact.");
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = true;
            playerObject = other.gameObject;
            Debug.Log($"[Interactable2D] {gameObject.name}: Player '{playerObject.name}' entered interaction range");
            OnEnterRange?.Invoke();
        }
        else
        {
            Debug.Log($"[Interactable2D] {gameObject.name}: Object '{other.name}' (Tag: '{other.tag}') entered trigger, but doesn't match Player tag '{playerTag}'");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = false;
            isPlayerFacing = false;
            Debug.Log($"[Interactable2D] {gameObject.name}: Player '{other.name}' exited interaction range");
            playerObject = null;

            if (interactUI != null)
            {
                interactUI.SetActive(false);
            }

            OnExitRange?.Invoke();

            if (panelIsOpen)
            {
                ClosePanel();
            }
        }
    }

    void CheckPlayerFacing2D()
    {
        if (playerObject == null) return;

        Vector2 playerPos = playerObject.transform.position;
        Vector2 objectPos = transform.position;
        Vector2 directionToObject = (objectPos - playerPos).normalized;

        Vector2 playerFacing = facingAxis == FacingAxis.Right
            ? (Vector2)playerObject.transform.right
            : (Vector2)playerObject.transform.up;

        float angle = Vector2.Angle(playerFacing, directionToObject);

        bool wasFacing = isPlayerFacing;
        isPlayerFacing = angle <= facingAngleThreshold;

        if (wasFacing != isPlayerFacing)
        {
            if (isPlayerFacing)
            {
                Debug.Log($"[Interactable2D] {gameObject.name}: Player is now facing the object (Angle: {angle:F1}°, Threshold: {facingAngleThreshold}°)");
            }
            else
            {
                Debug.Log($"[Interactable2D] {gameObject.name}: Player is no longer facing the object (Angle: {angle:F1}°, Threshold: {facingAngleThreshold}°)");
            }
        }

        if (interactUI != null)
        {
            interactUI.SetActive(isPlayerFacing);
        }
    }

    public void Interact()
    {
        Debug.Log($"[Interactable2D] {gameObject.name}: INTERACTION TRIGGERED by player '{playerObject?.name ?? "Unknown"}'");
        OnInteract?.Invoke();

        switch (interactionType)
        {
            case InteractionType.OpenPanel:
                OpenPanel();
                break;

            case InteractionType.CraftingSlotsOnly:
                if (craftingActivator == null)
                {
                    Debug.LogWarning($"[Interactable2D] {gameObject.name}: No SlotEnterActivator assigned.");
                    return;
                }
                var go = craftingActivator.gameObject;
                if (!go.activeInHierarchy) go.SetActive(true);
                Debug.Log($"[Interactable2D] {gameObject.name}: Crafting ready. Drag into input slot and press {craftingActivator.activateKey}.");
                break;
        }
    }

    private void OpenPanel()
    {
        if (panelToOpen == null)
        {
            Debug.LogWarning($"[Interactable2D] {gameObject.name}: No panelToOpen assigned.");
            return;
        }

        panelToOpen.SetActive(true);
        panelIsOpen = true;

        if (pauseOnOpen) Time.timeScale = 0f;

        if (manageCursorOnOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void ClosePanel()
    {
        if (panelToOpen == null) return;

        panelToOpen.SetActive(false);
        panelIsOpen = false;

        if (pauseOnOpen) Time.timeScale = 1f;

        if (manageCursorOnOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void SetInteractable(bool enabled)
    {
        this.enabled = enabled;
        if (triggerCollider2D != null)
        {
            triggerCollider2D.enabled = enabled;
        }
        if (interactUI != null && !enabled)
        {
            interactUI.SetActive(false);
        }
        Debug.Log($"[Interactable2D] {gameObject.name}: SetInteractable({enabled})");
    }
}
