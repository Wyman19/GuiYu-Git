using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUI : MonoBehaviour
{
    private Vector3 mTarget;
    private Vector3 mScreen;
    public float value;

    public float ContentWidth = 100;
    public float ContentHeight = 50;

    private Vector2 mPoint; //GUI坐标
    public float DestoryTime = 2.0f; //伤害数字消失时间
    // Start is called before the first frame update
    void Start()
    {
        mTarget = transform.position; //获取目标位置
        mScreen = Camera.main.WorldToScreenPoint(mTarget); //转化为屏幕位置
        mPoint = new Vector2(mScreen.x, Screen.height - mScreen.y);
        StartCoroutine("Free"); //开启一个协程
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * 0.5f * Time.deltaTime); //伤害数字上移效果
        mTarget = transform.position;
        mScreen = Camera.main.WorldToScreenPoint(mTarget);
        mPoint = new Vector2(mScreen.x, Screen.height - mScreen.y);//实时变化位置
    }

    void OnGUI()
    {
        if (mScreen.z > 0) //伤害数字显示
        {
            GUI.Label(new Rect(mPoint.x, mPoint.y, ContentWidth, ContentHeight), value.ToString());
        }

    }
    IEnumerator Free()  //协程，伤害数字时间一到消失
    {
        yield return new WaitForSeconds(DestoryTime);
        Destroy(this.gameObject);
    }
}
