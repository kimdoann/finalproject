using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent; //for returning to original position
    CanvasGroup canvasGroup;
    public float minDropDistance = 2f;
    public float maxDropDistance = 2f;
    float dropOffset = 3f;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f; //opacity during drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; //follow mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; //activate raycast
        canvasGroup.alpha = 1f; //back to full opacity
        Slot originalSlot = originalParent.GetComponent<Slot>();

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>(); //finds slot item dropped in
        if (dropSlot != null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if (dropSlot == null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
            }

            //if there's a viable slot under cursor
            if (dropSlot.currentItem != null)
            {
                //if slot full swap items
                dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                originalSlot.currentItem = dropSlot.currentItem;
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else
            {
                originalSlot.currentItem = null;
            }
            //Move item into drop slot
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        }
        else
        {
            if (!IsWithinInventory(eventData.position))
            {
                DropItem(originalSlot);
            }

            else
            {
                //drop outside inventory
                //if no new slot under cursor return to original
                transform.SetParent(originalParent);
            }
        }
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero; //center
    }
    bool IsWithinInventory(Vector2 mousePosition)
    {
        RectTransform inventoryRect = originalParent.parent.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, mousePosition);

    }
    void DropItem(Slot originalSlot)
    {
        Item item = GetComponent<Item>();
        if(item == null)
        {
            Debug.LogError("No item component");
            return;
        }
        originalSlot.currentItem = null;
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
        {
            Debug.LogError("Missing 'Player' tag");
            return;
        }
        //drop position
        Vector2 dropPosition = (Vector2)playerTransform.position + Vector2.right * dropOffset;
        Instantiate(item.worldItemPrefab, dropPosition, Quaternion.identity);
        Destroy(gameObject);
    }
}
