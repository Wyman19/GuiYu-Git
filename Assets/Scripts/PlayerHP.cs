using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : Hp
{

    private static PlayerHP _instance;
    public static PlayerHP Instance
    {
        get
        {
            
            return _instance;
        }
    }
    private void Awake()
    {

        if (_instance == null)
            _instance = this;
    }
}
