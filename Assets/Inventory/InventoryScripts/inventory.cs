using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Inventory", menuName= "Invontory/New Inventory")]
public class inventory : ScriptableObject
{
    //新建一个背包
    public int PacketNum;
    public Transform PacketPos;
    public Transform HoldPos;
    public Transform HandPos;
    public Transform HeadPos;
    public Transform BodyPos;
    public Transform LegPos;
    public Transform FootPos;
    public List<Item> itemList=new List<Item>();
    public List<GameObject> gameObjects = new List<GameObject>();
    public List<GameObject> copyGameObjects = new List<GameObject>();

    public void Clear()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] != null) itemList[i] = null;
            if (gameObjects[i] != null) Destroy(gameObjects[i]);
            if(copyGameObjects[i]!=null) Destroy(copyGameObjects[i]);
        }
    }
    /// <summary>
    /// 用于读档时加载玩家已装备的物品，从ScriptableObject中生成玩家的装备
    /// </summary>
    public void LoadEquip()
    {
        PacketNum = 0;
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] == null) continue;
            

            
            gameObjects[i] = Instantiate(itemList[i].itemPrefab, SwichItemHoldPos(itemList[i].armorType));
            
            gameObjects[i].transform.SetParent(SwichItemHoldPos(itemList[i].armorType));
            gameObjects[i].transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            gameObjects[i].transform.localPosition = new Vector3(0, 0, 0);
            gameObjects[i].transform.localRotation = Quaternion.identity;
            Debug.Log(this.name + "LoadEquip"+ gameObjects[i].transform.parent);
        }
    }
    /// <summary>
    /// 用于读档时加载玩家已装备的物品，从ScriptableObject中生成玩家的装备
    /// </summary>
    public void LoadWeapon()
    {
        PacketNum = 0;
        for (int i = 0; i < itemList.Count; i++)
        {

            if (itemList[i] == null) continue;
            bool value = true;
            if (i == 0) value = true;
            else value = false;

            
            gameObjects[i] = Instantiate(itemList[i].itemPrefab, HoldPos);
            

            gameObjects[i].transform.SetParent(HoldPos);
            gameObjects[i].transform.localPosition = new Vector3(0, 0, 0);
            gameObjects[i].transform.localRotation = Quaternion.identity;
            gameObjects[i].transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            gameObjects[i].SetActive(value);

            Destroy(copyGameObjects[i]);
            copyGameObjects[i] = Instantiate(gameObjects[i].gameObject, PacketPos);
            //设置object的位置
            copyGameObjects[i].transform.SetParent(PacketPos);
            copyGameObjects[i].transform.localPosition = new Vector3(0, 0, 0);
            copyGameObjects[i].transform.localRotation = Quaternion.identity;
            copyGameObjects[i].transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            copyGameObjects[i].SetActive(!value);

        }
        //bag.itemList[bag.PacketNum]=
    }
    public Transform SwichItemHoldPos(ArmorType armorType)
    {
        switch (armorType)
        {
            case ArmorType.Hand: return HandPos;
            case ArmorType.Head: return HeadPos;
            case ArmorType.Body: return BodyPos;
            case ArmorType.Leg: return LegPos;
            case ArmorType.Foot: return FootPos;
            case ArmorType.Other:return HoldPos;
            default: return null;
        }

    }
}
