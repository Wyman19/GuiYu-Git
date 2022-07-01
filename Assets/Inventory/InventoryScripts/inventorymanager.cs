using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inventorymanager : MonoBehaviour
{
    public inventory bag;
    public GameObject slotGrid;
    public slot slotPrefab;
    public Text itemInformantion;
    public List<slot> slots=new List<slot>();

    private void Start()
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
    public void CreateNewItem(Item item)
    {
        //新建一个slot用来装item的
        slot newItem = Instantiate(slotPrefab, slotGrid.transform.position, Quaternion.identity);
        newItem.gameObject.transform.SetParent(slotGrid.transform);
        //把item的值给slot，就能显示出来了
        newItem.slotItem = item;
        newItem.slotImage.sprite = item.itemImage;
        //newItem.slotNum.text = item.itemHeld.ToString();
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
        for (int i = 0; i < slotGrid.transform.childCount; i++)
        {
            if (slotGrid.transform.childCount == 0)
                break;
            //删除所有物品
            Destroy(slotGrid.transform.GetChild(i).gameObject);

        }
        foreach (Item value in bag.itemList)
        {
            //遍历字典+生成
            CreateNewItem(value);
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
            if (slots[i].slotGameObject != null)
            {
                slots[i].slotGameObject.SetActive(false);
                slots[i].slotCopyGameObject.SetActive(false);
            } 
            //把数据放入inventory
            bag.gameObjects[i] = slots[i].slotGameObject;
            bag.itemList[i] = slots[i].slotItem;
            bag.copyGameObjects[i] = slots[i].slotCopyGameObject;
        }
        
        //slots.ForEach(slot =>slot.slotGameObject?.SetActive(false));
    }
}
