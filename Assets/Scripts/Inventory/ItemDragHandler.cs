using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent; //for returning to original position
    CanvasGroup canvasGroup;

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

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>(); //finds slot item dropped in
        if (dropSlot != null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if(dropSlot == null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
            }
            Slot originalSlot = originalParent.GetComponent<Slot>();
            
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
            //if no new slot under cursor return to original
            transform.SetParent(originalParent);
        }
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero; //center
    }
}
