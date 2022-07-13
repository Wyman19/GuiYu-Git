using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemOnDrag : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public Transform originalParent;
    public Image originalImage;
    public slot originalParentSlot;

    public void OnBeginDrag(PointerEventData eventData)
    {

        originalParent = transform.parent;
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
        GameObject targetObject = eventData.pointerCurrentRaycast.gameObject;
        if (targetObject.name == "Image")
        {
            targetObject.TryGetComponent<Image>(out Image targetImage);
            targetObject.transform.parent.TryGetComponent<slot>(out slot targetslot);
            //检测是否可以交换
            if (targetslot.armorType != originalParentSlot.slotItem.armorType && originalParentSlot.slotItem.armorType == ArmorType.Other)
            {
                ReturnPosition();
                return;
            }
            //交换Slot
            originalParentSlot.SwitchSlot(targetslot);
            //交换Image
            Sprite sprite = originalImage.sprite;
            originalImage.sprite = targetImage.sprite;
            targetImage.sprite = sprite;
            //松手Image返回原来位置
            ReturnPosition();
            
            return;

        }
        else if (targetObject.name == "slot")
        {
            
            if (!targetObject.transform.Find("Image"))
            {
                ReturnPosition();
                return;
            }
            GameObject targetImageGameObject = targetObject.transform.GetChild(0).gameObject;
            targetImageGameObject.TryGetComponent<Image>(out Image targetImage);
            targetObject.TryGetComponent<slot>(out slot targetslot);
            //检测是否可以交换
            if (targetslot.armorType != originalParentSlot.slotItem.armorType&& originalParentSlot.slotItem.armorType==ArmorType.Other)
            {
                ReturnPosition();
                return;
            }
            //交换Slot
            originalParentSlot.SwitchSlot(targetslot);
            //交换Image
            Sprite sprite = originalImage.sprite;
            originalImage.sprite = targetImage.sprite;
            targetImage.sprite = sprite;
            targetImageGameObject.SetActive(true);
            transform.gameObject.SetActive(false);
            //松手Image返回原来位置
            ReturnPosition();
            
            return;
        }
        ReturnPosition();
        
    }
    void ReturnPosition()
    {
        transform.SetParent(originalParent);
        transform.position = originalParent.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }


}
