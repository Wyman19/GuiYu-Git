using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageRange : DamageRange
{


    private void Awake()
    {
        ThirdPersonController.Instance.TryGetComponent<Animator>(out userAnim);
        this._target = "Enemy";
        //this.damageValue = 1;
    }



}
