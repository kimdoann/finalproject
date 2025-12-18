using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class ItemRecipe
{
    public Sprite inputSprite;      // The sprite of the input item to match
    public string inputItemName;     // Alternative: match by item name
    public int inputItemID;          // Alternative: match by item ID (-1 to ignore)
    public Sprite outputSprite;      // The sprite to display in output
    public GameObject outputPrefab;  // Optional prefab to instantiate instead
}

public class SlotEnterActivator : MonoBehaviour
{
    [Header("Slots")]
    public Slot inputSlot;   // Slot where you drop the ingredient
    public Slot outputSlot;  // Slot where the result sprite will appear

    [Header("Activation")]
    public KeyCode activateKey = KeyCode.Return;
    public bool autoDisplayOnInput = false;  // Automatically display when item is placed

    [Header("Recipe System")]
    public List<ItemRecipe> recipes = new List<ItemRecipe>();  // List of item recipes

    [Header("Result Sprite Options (Fallback)")]
    public bool useSourceItemSprite = true;
    public Sprite resultSprite;
    public GameObject resultItemPrefab;

    [Header("Behavior")]
    public bool replaceOutputIfOccupied = true;
    public bool consumeInput = false;

    private GameObject lastInputItem = null;

    void Update()
    {
        // Check for automatic display when item is placed
        if (autoDisplayOnInput)
        {
            if (inputSlot != null && inputSlot.currentItem != lastInputItem)
            {
                if (inputSlot.currentItem != null)
                {
                    TryActivate();
                }
                else if (inputSlot.currentItem == null && outputSlot != null && outputSlot.currentItem != null)
                {
                    // Clear output when input is removed
                    ClearOutput();
                }
                lastInputItem = inputSlot.currentItem;
            }
        }

        // Manual activation with key
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

        // Try to find a matching recipe
        Sprite spriteToUse = null;
        GameObject prefabToUse = null;
        
        ItemRecipe matchedRecipe = FindMatchingRecipe(inputSlot.currentItem);
        
        if (matchedRecipe != null)
        {
            // Use recipe output
            spriteToUse = matchedRecipe.outputSprite;
            prefabToUse = matchedRecipe.outputPrefab;
            Debug.Log($"[SlotEnterActivator] Matched recipe for input item. Using recipe output sprite.");
        }
        else
        {
            // Fallback to original behavior
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
            prefabToUse = resultItemPrefab;
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
        if (prefabToUse != null)
        {
            go = Instantiate(prefabToUse, outputSlot.transform);
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

    private ItemRecipe FindMatchingRecipe(GameObject inputItem)
    {
        if (inputItem == null || recipes == null || recipes.Count == 0)
            return null;

        // Get input item components
        Image inputImage = inputItem.GetComponent<Image>();
        Item itemComponent = inputItem.GetComponent<Item>();
        Sprite inputSprite = inputImage != null ? inputImage.sprite : null;
        string inputName = itemComponent != null ? itemComponent.Name : "";
        int inputID = itemComponent != null ? itemComponent.ID : -1;

        // Try to find a matching recipe
        foreach (ItemRecipe recipe in recipes)
        {
            bool matches = false;

            // Match by sprite (if specified)
            if (recipe.inputSprite != null && inputSprite != null)
            {
                matches = recipe.inputSprite == inputSprite;
            }
            // Match by item name (if specified and sprite didn't match)
            else if (!string.IsNullOrEmpty(recipe.inputItemName) && !string.IsNullOrEmpty(inputName))
            {
                matches = recipe.inputItemName.Equals(inputName, System.StringComparison.OrdinalIgnoreCase);
            }
            // Match by item ID (if specified and other methods didn't match)
            else if (recipe.inputItemID >= 0 && inputID >= 0)
            {
                matches = recipe.inputItemID == inputID;
            }

            if (matches && recipe.outputSprite != null)
            {
                return recipe;
            }
        }

        return null;
    }

    private void ClearOutput()
    {
        if (outputSlot != null && outputSlot.currentItem != null)
        {
            Destroy(outputSlot.currentItem);
            outputSlot.currentItem = null;
        }
    }
}