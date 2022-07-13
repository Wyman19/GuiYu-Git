using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveManage : MonoBehaviour
{
    public static SaveManage Instance { get; private set; }
    [SerializeField] public GameObject Player;
    [SerializeField] public inventory myBag;
    [SerializeField] public inventory myPacket_R;
    [SerializeField] public inventory myPacket_L;
    [SerializeField] public inventory myPacket_Armor;

    //需要保存的数据
    //[System.Serializable] struct SaveData
    //{
    //    public Vector3 playerPos;
    //    public List<Item> myBagItemList;
    //    public List<Item> myPacket_R_ItemList;
    //    public List<Item> myPacket_L_ItemList;
    //    public List<Item> myPacket_Armor_ItemList;
    //}
    //SaveData saveData;

   [System.Serializable]
   class SaveData
    {
        public int scenes;
        public Vector3 playerPos;
        public List<Item> myBagItemList;
        public List<Item> myPacket_R_ItemList;
        public List<Item> myPacket_L_ItemList;
        public List<Item> myPacket_Armor_ItemList;
    }
    SaveData saveData = new SaveData();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {

    }

    public void Save(string saveDataName)
    {
        //保存路径
        var path = Path.Combine(Application.persistentDataPath , "game_SaveData", saveDataName);
        Debug.Log(Application.persistentDataPath);
        //检测是否存在
        if(!Directory.Exists(Path.Combine(Application.persistentDataPath, "game_SaveData")))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "game_SaveData"));
        }
        
        //获取最新的数据
        CreateSaveData();
        var json = JsonUtility.ToJson(saveData);
        //写入
        File.WriteAllText(path, json);
        
    }
    public void Load(string saveDataName)
    {
        var path = Path.Combine(Application.persistentDataPath ,"game_SaveData", saveDataName);
        Debug.Log("Loading："+path);
        if (File.Exists(path))
        {
            //FileStream file = File.Open(Application.persistentDataPath + "/PlayerData.dat",FileMode.Open);
            
            var file = File.ReadAllText(path);
            var data=JsonUtility.FromJson<SaveData>(file);
            Debug.Log(data.myPacket_R_ItemList.Count);
            //把数据写入游戏中
            LoadSaveData(data);
           
            //初始化UI
            UIManager.Instance.InitializePanelStack();
            UIManager.Instance.InitializePanelDict();
            PauseSetting._isPause = false;

        }
    }
    public void DeleteSaveData(string saveDataName)
    {
        var path = Path.Combine(Application.persistentDataPath, "game_SaveData", saveDataName);
        if (File.Exists(path))
        {
            Debug.Log("删除存档" + saveDataName);
            File.Delete(path);
        }
       
    }
    void CreateSaveData()
    {
        saveData.scenes = SceneManager.GetActiveScene().buildIndex;
        saveData.playerPos = Player.transform.position;
        saveData.myBagItemList = myBag.itemList;
        saveData.myPacket_R_ItemList = myPacket_R.itemList;
        saveData.myPacket_L_ItemList = myPacket_L.itemList;
        saveData.myPacket_Armor_ItemList = myPacket_Armor.itemList;
    }
    
    void LoadSaveData(SaveData data)
    {
        myBag.Clear();
        myPacket_L.Clear();
        myPacket_R.Clear();
        myPacket_Armor.Clear();
        SceneManager.LoadScene(data.scenes);
        Player.transform.position = data.playerPos;
        myBag.itemList= data.myBagItemList;
        myPacket_R.itemList= data.myPacket_R_ItemList;
        myPacket_L.itemList= data.myPacket_L_ItemList;
        myPacket_Armor.itemList= data.myPacket_Armor_ItemList;
        myPacket_R.LoadWeapon();
        myPacket_L.LoadWeapon();
        myPacket_Armor.LoadEquip();
    }

    
}
