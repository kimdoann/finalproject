using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    public float maxDistance = 3f;
    public LayerMask interactableMask;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (TryRaycastInteract(out Interactable interactable))
            {
                interactable.Interact();
            }
        }
    }

    bool TryRaycastInteract(out Interactable interactable)
    {
        interactable = null;
        var cam = Camera.main;
        var ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if (Physics.Raycast(ray, out var hit, maxDistance, interactableMask))
        {
            interactable = hit.collider.GetComponentInParent<Interactable>();
            return interactable != null;
        }
        return false;
    }
}
