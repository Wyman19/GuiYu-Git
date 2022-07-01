using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hp : MonoBehaviour
{
    public Image hp;   
    public GameObject damageUI;
    public Vector3 statusBar;
    public float MaxHp=100;
    public float ResidueHp = 100;
    [Range(0, 1)]
    public float time;
    private void Awake()
    {
        ResidueHp = MaxHp;
    }
    private void Start()
    {
        hp= transform.Find("StateCanva").Find("Frame").Find("State").Find("hp").gameObject.GetComponent<Image>();
        statusBar = transform.Find("StateCanva").Find("StatusBar").gameObject.transform.position;
        damageUI = Resources.Load("DamageUI") as GameObject;
        

    }
    private void Update()
    {
        time +=Time.deltaTime;
    }

    public void GetDamage(float damage )
    {
        if(time >= 0.5f)
        {
            
            time = 0;
            ResidueHp-=damage;
            Death();
            hp.fillAmount = ResidueHp/100;
            //œ‘ æ…À∫¶ ˝÷µ
            GameObject DamageText = (GameObject)Instantiate(damageUI, statusBar, Quaternion.identity);
            DamageText.GetComponent<DamageUI>().value = damage;
        }
        
    }


    public void ReplyHp(float ReplyValue)
    {
        ResidueHp += ReplyValue;
        if (ResidueHp > MaxHp) ResidueHp = MaxHp;
    }
    public void Death()
    {
        if(ResidueHp <= 0) GameObject.Destroy(gameObject);
    }

}
