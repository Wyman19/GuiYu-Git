using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DamageRange : MonoBehaviour
{
    public Item thisItem;

    public Animator userAnim;
    public GameObject blood;
    public bool isGiveDamage;
    public float damageValue;
    public float pos_Up;
    public float length;
    public LayerMask target;

    protected string _target;
    private void Start()
    {
        blood = Resources.Load("FX_BloodSplat_Small_01") as GameObject;
        var t2d= AssetPreview.GetAssetPreview(transform.gameObject);
        thisItem.itemImage = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
    }

    private void Update()
    {

        Debug.DrawRay(transform.position + transform.up * pos_Up, transform.up*length);
    }
    
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(_target)&& isGiveDamage)
        {
            userAnim.speed = 0.3f;
            collision.gameObject.TryGetComponent<Hp>(out Hp hp); 
            hp.GetDamage(damageValue);
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                //ÔÚÅö×²µã³öÑª
                Instantiate(blood, collision.contacts[i].point, Quaternion.identity);
            }
        }  
    }
    private void OnCollisionExit(Collision collision)
    {
        userAnim.speed = 1;
    }


}
