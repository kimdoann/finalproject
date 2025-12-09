using UnityEngine;
using UnityEngine.UI;

public class SlotEnterActivator : MonoBehaviour
{
    [Header("Slots")]
    public Slot inputSlot;   // Slot where you drop the ingredient
    public Slot outputSlot;  // Slot where the result sprite will appear

    [Header("Activation")]
    public KeyCode activateKey = KeyCode.Return;

    [Header("Result Sprite Options")]
    public bool useSourceItemSprite = true;
    public Sprite resultSprite;
    public GameObject resultItemPrefab;

    [Header("Behavior")]
    public bool replaceOutputIfOccupied = true;
    public bool consumeInput = false;

    void Update()
    {
        if (Input.GetKeyDown(activateKey))
        {
            TryActivate();
        }
    }

    public void TryActivate()
    {
        if (inputSlot == null || outputSlot == null)
        {
            Debug.LogWarning("[SlotEnterActivator] Slots not assigned.");
            return;
        }

        if (inputSlot.currentItem == null)
        {
            Debug.Log("[SlotEnterActivator] No ingredient in input slot.");
            return;
        }

        if (outputSlot.currentItem != null && !replaceOutputIfOccupied)
        {
            Debug.Log("[SlotEnterActivator] Output slot occupied and replace is disabled.");
            return;
        }

        Sprite spriteToUse = null;
        if (useSourceItemSprite)
        {
            var srcImg = inputSlot.currentItem.GetComponent<Image>();
            if (srcImg == null || srcImg.sprite == null)
            {
                Debug.LogWarning("[SlotEnterActivator] Input item has no Image or Sprite. Falling back to resultSprite.");
                spriteToUse = resultSprite;
            }
            else
            {
                spriteToUse = srcImg.sprite;
            }
        }
        else
        {
            spriteToUse = resultSprite;
        }

        if (spriteToUse == null)
        {
            Debug.LogWarning("[SlotEnterActivator] No sprite available to display.");
            return;
        }

        if (outputSlot.currentItem != null)
        {
            Destroy(outputSlot.currentItem);
            outputSlot.currentItem = null;
        }

        GameObject go;
        if (resultItemPrefab != null)
        {
            go = Instantiate(resultItemPrefab, outputSlot.transform);
        }
        else
        {
            go = new GameObject("ResultItem", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            go.transform.SetParent(outputSlot.transform, false);
        }

        var img = go.GetComponent<Image>();
        if (img == null) img = go.AddComponent<Image>();
        img.sprite = spriteToUse;
        img.preserveAspect = true;

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;

        outputSlot.currentItem = go;

        if (consumeInput && inputSlot.currentItem != null)
        {
            Destroy(inputSlot.currentItem);
            inputSlot.currentItem = null;
        }
    }
}