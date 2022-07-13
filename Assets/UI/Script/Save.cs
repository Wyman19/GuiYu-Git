
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Save : BasePanel
{
    public string path;
    public RectTransform content;
    public GameObject newSavePanel;
    public GameObject OverwritePanel;
    public GameObject savePrefab;
    public Text originalSaveNume;
    public Text saveNume;
    public Text originalOverwriteNume;
    public Text OverwriteNume;
    public int showSaveFileNum;
    public void OnClickReturnButton()
    {
        UIManager.Instance.PopPanel();
    }
    public void OnClickNewSave()
    {
        originalSaveNume.text = "请输入新建存档的名字";
        newSavePanel.SetActive(true);
    }
    public void OnClickReturnNewSave()
    {
        newSavePanel.SetActive(false);
    }public void OnClickReturnOverwriteSave()
    {
        OverwritePanel.SetActive(false);
    }
    public void OnClickOverwriteSave(GameObject gameObject)
    {
        gameObject.transform.GetChild(0).TryGetComponent<Text>(out var saveText);
        originalOverwriteNume.text = saveText.text;
        OverwritePanel.SetActive(true);
    }
    public void OnClickConfirmLoad()
    {
        SaveManage.Instance.Load(originalOverwriteNume.text);
        OverwritePanel.SetActive(false);
    }
    public void OnClickConfirmDelete()
    {
        //删除原来的
        SaveManage.Instance.DeleteSaveData(originalOverwriteNume.text);
        RefreshContent();
        OverwritePanel.SetActive(false);
        //SaveManage.Instance.Save(saveNume.text);
    }
    public void OnClickConfirmSave()
    {
        SaveManage.Instance.Save(saveNume.text==null? originalSaveNume.text: saveNume.text);
        GameObject newSave = Instantiate(savePrefab, content.transform);
        newSave.TryGetComponent<RectTransform>(out var rectTransform);
        rectTransform.anchoredPosition = new Vector3(0, -(showSaveFileNum * 120 + 60), 0);
        
        newSave.transform.GetChild(0).TryGetComponent<Text>(out var newSaveText);
        newSaveText.text = saveNume.text == null ? originalSaveNume.text : saveNume.text;
        newSave.GetComponent<Button>().onClick.AddListener(() => OnClickOverwriteSave(newSave));
        showSaveFileNum++;
        content.sizeDelta = new Vector2(0, 120 * showSaveFileNum);
        newSavePanel.SetActive(false);
        Debug.Log(rectTransform.localPosition);
    }
    public override void OnEnter()
    {
        RefreshContent();


    }
    void RefreshContent()
    {
        showSaveFileNum = 0;
        path = Path.Combine(Application.persistentDataPath, "game_SaveData");
        base.OnEnter();
        //清空存档显示内容
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        DirectoryInfo folder = new DirectoryInfo(path);
        //获取所有存档
        foreach (FileInfo file in folder.GetFiles("*"))
        {
            showSaveFileNum++;

            GameObject newSave = Instantiate(savePrefab, content.transform);
            newSave.TryGetComponent<RectTransform>(out var rectTransform);
            rectTransform.anchoredPosition = new Vector3(0, -((showSaveFileNum - 1) * 120 + 60), 0);
            newSave.transform.GetChild(0).TryGetComponent<Text>(out var newSaveText);
            newSaveText.text = file.Name;
            Debug.Log(file.Name);
            newSave.GetComponent<Button>().onClick.AddListener(() => OnClickOverwriteSave(newSave));
        }
        //调整大小
        content.sizeDelta = new Vector2(0, 120 * showSaveFileNum);
    }
}
