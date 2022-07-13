using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Medicine_HP : MonoBehaviour
{
    public Item thisItem;

    public Hp userHp;
    public int maxDegree;
    public int residueDegree;
    public int replyNum;

    private void Awake()
    {
        userHp = PlayerHP.Instance;
        residueDegree = maxDegree;
    }
    private void Start()
    {
        var t2d = AssetPreview.GetAssetPreview(transform.gameObject);
        thisItem.itemImage = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
    }
    /// <summary>
    /// ʹ��ҩƷ��Ѫ
    /// </summary>
    public void Use()
    {
        if (residueDegree > 0)
        {
            residueDegree--;
            userHp.ReplyHp(replyNum);
        } 
    }
    /// <summary>
    /// �ظ�ҩƷʹ�ô����������Ƿ�ظ��ɹ�
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool ReplyDegree(int value)
    {
        if (residueDegree == maxDegree) return false;
        residueDegree += value;
        if(residueDegree > maxDegree) residueDegree = maxDegree;
        return true;

    }
}
