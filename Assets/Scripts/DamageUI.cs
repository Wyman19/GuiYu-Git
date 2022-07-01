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

    private Vector2 mPoint; //GUI����
    public float DestoryTime = 2.0f; //�˺�������ʧʱ��
    // Start is called before the first frame update
    void Start()
    {
        mTarget = transform.position; //��ȡĿ��λ��
        mScreen = Camera.main.WorldToScreenPoint(mTarget); //ת��Ϊ��Ļλ��
        mPoint = new Vector2(mScreen.x, Screen.height - mScreen.y);
        StartCoroutine("Free"); //����һ��Э��
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * 0.5f * Time.deltaTime); //�˺���������Ч��
        mTarget = transform.position;
        mScreen = Camera.main.WorldToScreenPoint(mTarget);
        mPoint = new Vector2(mScreen.x, Screen.height - mScreen.y);//ʵʱ�仯λ��
    }

    void OnGUI()
    {
        if (mScreen.z > 0) //�˺�������ʾ
        {
            GUI.Label(new Rect(mPoint.x, mPoint.y, ContentWidth, ContentHeight), value.ToString());
        }

    }
    IEnumerator Free()  //Э�̣��˺�����ʱ��һ����ʧ
    {
        yield return new WaitForSeconds(DestoryTime);
        Destroy(this.gameObject);
    }
}
