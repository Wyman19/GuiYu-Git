using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="New Item",menuName ="Invontory/New Item")]
public class Item : ScriptableObject
{
    //һ����Ʒ������
    public int id;
    public string itemName;
    public Sprite itemImage;
    [TextArea]
    public string itemInfo;
    public ArmorType armorType;
    

 
}
