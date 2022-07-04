using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemOnDrag : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public Transform origialParent;
    public Vector3 originalPosition;
    public Image originalImage;
    public slot originalParentSlot;

    public void OnBeginDrag(PointerEventData eventData)
    {
        origialParent = transform.parent;
        originalPosition = transform.position;
        originalImage = GetComponent<Image>();
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

        if (eventData.pointerCurrentRaycast.gameObject.name == "Image")
        {
            eventData.pointerCurrentRaycast.gameObject.TryGetComponent<Image>(out Image targetImage);
            eventData.pointerCurrentRaycast.gameObject.transform.parent.TryGetComponent<slot>(out slot targetslot);
            //交换Slot
            originalParentSlot.SwitchSlot(targetslot);
            //交换Image
            Sprite sprite = originalImage.sprite;
            originalImage.sprite = targetImage.sprite;
            targetImage.sprite = sprite;
            //松手Image返回原来位置
            transform.SetParent(origialParent);
            transform.position = originalPosition;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            return;

        }
        else if (eventData.pointerCurrentRaycast.gameObject.name == "slot")
        {
            GameObject targetImageGameObject = eventData.pointerCurrentRaycast.gameObject.transform.GetChild(0).gameObject;
            targetImageGameObject.TryGetComponent<Image>(out Image targetImage);
            eventData.pointerCurrentRaycast.gameObject.TryGetComponent<slot>(out slot targetslot);
            //交换Slot
            originalParentSlot.SwitchSlot(targetslot);
            //交换Image
            Sprite sprite = originalImage.sprite;
            originalImage.sprite = targetImage.sprite;
            targetImage.sprite = sprite;
            targetImageGameObject.SetActive(true);
            transform.gameObject.SetActive(false);
            //松手Image返回原来位置
            transform.SetParent(origialParent);
            transform.position = originalPosition;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            return;
        }
        transform.SetParent(origialParent);
        transform.position = originalPosition;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }


}
