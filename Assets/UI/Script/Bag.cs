using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bag : BasePanel
{
    public static bool _isPause;
    public inventorymanager Packet_Left;
    public inventorymanager Packet_Right;
    public inventorymanager Packet_Armor;
    public inventorymanager MyBag;
    private void Start()
    {
        
        Packet_Left=transform.Find("Packet_Left").gameObject.GetComponent<inventorymanager>();
        Packet_Right=transform.Find("Packet_Right").gameObject.GetComponent<inventorymanager>();
        MyBag=transform.Find("Bag").gameObject.GetComponent<inventorymanager>();
        //Packet_Left.RefreshItemInPacker();
        //Packet_Right.RefreshItemInPacker();

    }

    public override void OnEnter()
    {
        base.OnEnter();
        Packet_Left.RefreshItemInPacker();
        Packet_Right.RefreshItemInPacker();
        Packet_Armor.RefreshItemInArmor();
        MyBag.RefreshItem();
        _isPause = true;

    }
    public override void OnExit()
    {
        base.OnExit();
        Packet_Left.SaveBag_Packet();
        Packet_Right.SaveBag_Packet();
        Packet_Armor.SaveBag_Armor();
        MyBag.SaveBag();
        _isPause = false;
    }


}

