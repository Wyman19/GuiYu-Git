using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : BasePanel
{
    [SerializeField] public inventory myBag;
    [SerializeField] public inventory myPacket_R;
    [SerializeField] public inventory myPacket_L;
    [SerializeField] public inventory myPacket_Armor;
    public override void OnEnter()
    {
        base.OnEnter();
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnClickNewStartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //要pop所有的Panel
        UIManager.Instance.PopAllPanel();
        //初始化字典
        UIManager.Instance.InitializePanelDict();
        myBag.Clear();
        myPacket_R.Clear();
        myPacket_R.Clear();
        myPacket_Armor.Clear();
        //UIManager.Instance.PushPanel(UIPanelType.SystemSettingPanel);
    }

    public void OnClickContinueButton()
    {
        UIManager.Instance.PushPanel(UIPanelType.Load);
        //UIManager.Instance.PushPanel(UIPanelType.StorePanel);
    }

    public void OnClickSettingButton()
    {
        UIManager.Instance.PushPanel(UIPanelType.Setting);
        //UIManager.Instance.PushPanel(UIPanelType.PausePanel);
    }
    public void OnClickQuiteButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        //UIManager.Instance.PushPanel(UIPanelType.PausePanel);
    }
}
