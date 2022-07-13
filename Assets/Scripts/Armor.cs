using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Armor : MonoBehaviour
{
    public Item thisItem;
    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        var t2d = AssetPreview.GetAssetPreview(transform.gameObject);
        thisItem.itemImage = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
