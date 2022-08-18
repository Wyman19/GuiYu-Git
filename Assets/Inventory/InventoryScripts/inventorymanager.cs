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
    /// 刷新背包，从ScriptableObject获取数据给slot
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
                slots[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        }

    }
    /// <summary>
    /// 刷新护甲装备栏，从ScriptableObject获取数据给slot
    /// </summary>
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
                slots[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
    /// <summary>
    /// 刷新左右手的装备栏，从ScriptableObject获取数据给slot
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
    /// <summary>
    /// 把slot的数据给ScriptableObject，并对卸下与装备的物品做相应的效果
    /// </summary>
    public void SaveBag_Packet()
    {
        Debug.Log(this.name + "_SaveBag_Packet");
        bag.PacketNum = 0;
        FSM.Instance.parameter.PacketNum_L = 0;
        FSM.Instance.parameter.PacketNum_R = 0;
        for (int i=0; i < slots.Count; i++)
        {
            //Debug.Log("SaceBag_Packet");
            //跳过空的格子
            //if (slots[i].slotItem == bag.itemList[i]) continue;
            //把数据放入inventory
            bag.gameObjects[i] = slots[i].slotGameObject;
            bag.itemList[i] = slots[i].slotItem;
            bag.copyGameObjects[i] = slots[i].slotCopyGameObject;
            if (slots[i].slotItem == null) continue;
            bool value = true;
            if (i == 0) value = true;
            else value = false;
            
            if (bag.gameObjects[i] == null)
            {
                bag.gameObjects[i] = Instantiate(bag.itemList[i].itemPrefab, bag.HoldPos);
            }
            Debug.Log(this.name + "_SaveBag_Packet");
            bag.gameObjects[i].transform.SetParent(bag.HoldPos);
            bag.gameObjects[i].transform.localPosition = new Vector3(0, 0, 0);
            bag.gameObjects[i].transform.localRotation = Quaternion.identity;
            bag.gameObjects[i].transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            bag.gameObjects[i].SetActive(value);

            Destroy(bag.copyGameObjects[i]);
            bag.copyGameObjects[i] = Instantiate(bag.gameObjects[i].gameObject, bag.PacketPos);
            //设置object的位置
            bag.copyGameObjects[i].transform.SetParent(bag.PacketPos);
            bag.copyGameObjects[i].transform.localPosition = new Vector3(0, 0, 0);
            bag.copyGameObjects[i].transform.localRotation = Quaternion.identity;
            bag.copyGameObjects[i].transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            bag.copyGameObjects[i].SetActive(!value);

        }
        //bag.itemList[bag.PacketNum]=
    }
    /// <summary>
    /// 把slot的数据给ScriptableObject，并对卸下与装备的物品做相应的效果
    /// </summary>
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
    /// <summary>
    /// 把slot的数据给ScriptableObject，并对卸下与装备的物品做相应的效果
    /// </summary>
    public void SaveBag_Armor()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            //Debug.Log("SaceBag_Packet");
            //跳过空的格子
            //if (slots[i].slotItem == bag.itemList[i]) continue;
            //把数据放入inventory
            bag.gameObjects[i] = slots[i].slotGameObject;
            bag.itemList[i] = slots[i].slotItem;
            bag.copyGameObjects[i] = slots[i].slotCopyGameObject;
            if (slots[i].slotItem == null) continue;
            

            if (bag.gameObjects[i] == null)
            {
                bag.gameObjects[i] = Instantiate(bag.itemList[i].itemPrefab, SwichItemHoldPos(bag.itemList[i].armorType));
            }

            bag.gameObjects[i].transform.SetParent(SwichItemHoldPos(bag.itemList[i].armorType));
            bag.gameObjects[i].transform.localPosition = new Vector3(0, 0, 0);
            bag.gameObjects[i].transform.localRotation = Quaternion.identity;
            bag.gameObjects[i].transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            bag.gameObjects[i].SetActive(true);

           

        }
    }

    public Transform SwichItemHoldPos(ArmorType armorType)
    {
        switch(armorType)
        {
            case ArmorType.Hand: return bag.HandPos;
            case ArmorType.Head: return bag.HeadPos;
            case ArmorType.Body: return bag.BodyPos;
            case ArmorType.Leg: return bag.LegPos;
            case ArmorType.Foot: return bag.FootPos;
            default: return null;
        }
            
    }
}
