using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Inventory", menuName= "Invontory/New Inventory")]
public class inventory : ScriptableObject
{
    //�½�һ������
    public int PacketNum;
    public Transform PacketPos;
    public List<Item> itemList=new List<Item>();
    public List<GameObject> gameObjects = new List<GameObject>();
    public List<GameObject> copyGameObjects = new List<GameObject>();
}
