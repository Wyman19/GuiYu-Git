using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class slot : MonoBehaviour
{
    //背包的一个格
    public Item slotItem;
    public GameObject slotGameObject;
    public GameObject slotCopyGameObject;
    public Image slotImage;
    public Text slotNum;
    /// <summary>
    /// Switches the slot,传入目标slot
    /// </summary>
    /// <param name="slot">The slot.</param>
    public void SwitchSlot(slot slot)
    {
        slot orightslot = GetComponent<slot>();

        Item orightslotItem = orightslot.slotItem;
        GameObject orightslotGameObject = orightslot.slotGameObject;
        GameObject orightslotCopyGameObject = orightslot.slotCopyGameObject;

        slotItem = slot.slotItem;
        slotGameObject = slot.slotGameObject;
        slotCopyGameObject = slot.slotCopyGameObject;

        slot.slotItem = orightslotItem;
        slot.slotGameObject = orightslotGameObject;
        slot.slotCopyGameObject = orightslotCopyGameObject;

    }
}
