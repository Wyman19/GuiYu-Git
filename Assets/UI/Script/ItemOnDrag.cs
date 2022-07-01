using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemOnDrag : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public Transform originalParent;
    public slot originalParentSlot;
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalParentSlot =transform.parent.GetComponent<slot>();
        Debug.Log(originalParentSlot.slotItem);
        transform.SetParent(transform.parent.parent.parent);
        transform.position = eventData.position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        Debug.Log(eventData.pointerCurrentRaycast.gameObject.name);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Transform targetTransform = eventData.pointerCurrentRaycast.gameObject.transform;
        //if (eventData.pointerCurrentRaycast.gameObject.name == "Image")
        //{
        //    //交换GameObject
        //    var targetSlotGameObject = targetTransform.parent.GetComponent<slot>().slotGameObject;
        //    targetTransform.parent.GetComponent<slot>().slotGameObject = originalParentSlot.slotGameObject;
        //    originalParentSlot.slotGameObject = targetSlotGameObject;
        //    //交换CopyGameObject
        //    var targetSlotCopyGameObject = targetTransform.parent.GetComponent<slot>().slotCopyGameObject;
        //    targetTransform.parent.GetComponent<slot>().slotCopyGameObject = originalParentSlot.slotCopyGameObject;
        //    originalParentSlot.slotCopyGameObject = targetSlotCopyGameObject;
        //    //交换slot
        //    var targetSlotItem = targetTransform.parent.GetComponent<slot>().slotItem;
        //    targetTransform.parent.GetComponent<slot>().slotItem = originalParentSlot.slotItem;
        //    originalParentSlot.slotItem = targetSlotItem;
        //    //交换image
        //    transform.SetParent (targetTransform.parent);
        //    transform.position = targetTransform.parent.position;
        //    targetTransform.position = originalParent.position;
        //    targetTransform.SetParent(originalParent);
        //    GetComponent<CanvasGroup>().blocksRaycasts = true;
        //    return;
        //}if (eventData.pointerCurrentRaycast.gameObject.name == "slot")
        //{
        //    //交换GameObject
        //    var targetSlotGameObject = targetTransform.GetComponent<slot>().slotGameObject;
        //    targetTransform.GetComponent<slot>().slotGameObject = originalParentSlot.slotGameObject;
        //    originalParentSlot.slotGameObject = targetSlotGameObject;
        //    //交换CopyGameObject
        //    var targetSlotCopyGameObject = targetTransform.GetComponent<slot>().slotCopyGameObject;
        //    targetTransform.GetComponent<slot>().slotCopyGameObject = originalParentSlot.slotCopyGameObject;
        //    originalParentSlot.slotCopyGameObject = targetSlotCopyGameObject;
        //    //交换slot
        //    var targetSlotItem = targetTransform.GetComponent<slot>().slotItem;
        //    targetTransform.GetComponent<slot>().slotItem = originalParentSlot.slotItem;
        //    originalParentSlot.slotItem = targetSlotItem;
        //    //交换image
        //    targetTransform.GetChild(0).position = originalParent.position;
        //    targetTransform.GetChild(0).SetParent(originalParent);
        //    transform.SetParent (targetTransform);
        //    transform.position = targetTransform.position;
        //    GetComponent<CanvasGroup>().blocksRaycasts = true;
        //    return;
        //}
        Transform targetTransform = eventData.pointerCurrentRaycast.gameObject.transform;
        if (eventData.pointerCurrentRaycast.gameObject.name == "Image")
        {
            transform.SetParent( targetTransform.parent);
            targetTransform.SetParent ( originalParent);
            //transform.SetParent ( t);

        }
        else if (eventData.pointerCurrentRaycast.gameObject.name == "slot")
        {

        }
        transform.SetParent(targetTransform);
        transform.position = targetTransform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }


}
