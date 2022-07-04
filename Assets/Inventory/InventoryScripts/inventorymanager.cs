using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inventorymanager : MonoBehaviour
{
    public inventory bag;
    public GameObject slotPrefab;
    public Text itemInformantion;
    public List<slot> slots=new List<slot>();

    private void Awake()
    {
        GetSlot();
    }
    public void GetSlot()
    {
        var newSlots = transform.GetComponentsInChildren<slot>();
        for (int i = 0; i < newSlots.Length; i++)
        {
            slots[i] = newSlots[i];
        }
        
    }
    public void AddSlot(int AddNum)
    {

    }
    public void CreateNewItemInPacker(int packetNum, int index)
    {
        slots[index].slotItem = bag.itemList[packetNum];
        slots[index].slotImage.sprite = bag.itemList[packetNum].itemImage;
        slots[index].slotGameObject=bag.gameObjects[packetNum];
        slots[index].slotCopyGameObject =bag.copyGameObjects[packetNum];
        slots[index].transform.GetChild(0).gameObject.SetActive(true);
        //slots[PacketNum].slotNum.text = item.itemHeld.ToString();
    }
    /// <summary>
    /// 刷新背包
    /// </summary>
    public void RefreshItem()
    {
        //GetSlot();
        for (int i = 0; i < slots.Count; i++)
        {
            //删除物品

            slots[i].slotItem = null;
            slots[i].slotGameObject = null;
            slots[i].slotCopyGameObject = null;
            slots[i].slotImage.sprite = null;

            if (bag.itemList[i] != null)
            {
                slots[i].slotItem = bag.itemList[i];
                slots[i].slotGameObject = bag.gameObjects[i];
                slots[i].slotCopyGameObject = bag.copyGameObjects[i];
                slots[i].slotImage.sprite = bag.itemList[i].itemImage;
            }
        }

    }
    public void RefreshItemInArmor()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            //删除物品

            slots[i].slotItem = null;
            slots[i].slotGameObject = null;
            slots[i].slotCopyGameObject = null;
            slots[i].slotImage.sprite = null;

            if (bag.itemList[i] != null)
            {
                slots[i].slotItem = bag.itemList[i];
                slots[i].slotGameObject = bag.gameObjects[i];
                slots[i].slotCopyGameObject = bag.copyGameObjects[i];
                slots[i].slotImage.sprite = bag.itemList[i].itemImage;
            }
        }
    }
    /// <summary>
    /// 刷新左右手的装备
    /// </summary>
    /// <param name="PacketNum"></param>
    public void RefreshItemInPacker()
    {
        if (bag.itemList[bag.PacketNum] != null)
        {
            CreateNewItemInPacker(bag.PacketNum, 0);
        }
        else 
        {
            slots[0].slotGameObject = null;
            slots[0].slotItem = null;
            slots[0].transform.GetChild(0).gameObject.SetActive(false);
        }

        
        if (bag.itemList[(bag.PacketNum + 1) % 2] != null) CreateNewItemInPacker((bag.PacketNum + 1)%2,1);
        else
        {
            slots[1].slotGameObject = null;
            slots[1].slotItem = null;
            slots[1].transform.GetChild(0).gameObject.SetActive(false);
        }


    }
    public void SaveBag_Packet()
    {
        bag.PacketNum = 0; 
        for (int i=0; i < slots.Count; i++)
        {
            //把数据放入inventory
            bag.gameObjects[i] = slots[i].slotGameObject;
            bag.itemList[i] = slots[i].slotItem;
            bag.copyGameObjects[i] = slots[i].slotCopyGameObject;
            if (bag.gameObjects[i] != null)
            {
                bool value = true;
                if (i==0) value = true;
                else value = false;
                //Destroy(bag.copyGameObjects[i]);
                //bag.copyGameObjects[i] = Instantiate(bag.gameObjects[i], bag.PacketPos);
                bag.gameObjects[i].SetActive(value);
                if (bag.copyGameObjects[i] == null)
                    bag.copyGameObjects[i] = Instantiate(bag.gameObjects[i], bag.PacketPos);
                bag.copyGameObjects[i].SetActive(!value);
            }
        }
        //bag.itemList[bag.PacketNum]=
    }
    public void SaveBag()
    {
        //把放到背包的物品隐藏
        for (int i = 0; i < slots.Count; i++)
        {
            if(slots[i].slotGameObject != null)
            slots[i].slotGameObject.SetActive(false);
            if(slots[i].slotCopyGameObject != null)
            slots[i].slotCopyGameObject.SetActive(false);
            
            //把数据放入inventory
            bag.gameObjects[i] = slots[i].slotGameObject;
            bag.itemList[i] = slots[i].slotItem;
            bag.copyGameObjects[i] = slots[i].slotCopyGameObject;
        }
        
        //slots.ForEach(slot =>slot.slotGameObject?.SetActive(false));
    }
    public void SaveBag_Armor()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            //隐藏手的物品
            if (slots[i].slotGameObject != null)
                slots[i].slotGameObject.SetActive(false);
            if (slots[i].slotCopyGameObject != null)
                slots[i].slotCopyGameObject.SetActive(false);
            //在身上显示装备

            //把数据放入inventory
            bag.gameObjects[i] = slots[i].slotGameObject;
            bag.itemList[i] = slots[i].slotItem;
            bag.copyGameObjects[i] = slots[i].slotCopyGameObject;
        }
    }
}
