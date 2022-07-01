using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseSetting : BasePanel
{
	public static bool _isPause;
    ControlInstructions _controlInstructions;
    private void Start()
    {
        //找UI
        _controlInstructions = transform.parent.Find("ControlInstructions").gameObject.GetComponent<ControlInstructions>();
        //添加事件
        GameObject.Find("Control instructions").GetComponent<Toggle>().onValueChanged.AddListener(isOn => OnClickControlInstructionsToggle(isOn));
    }
    public void OnClickControlInstructionsToggle(bool isOn)
    {
        if (isOn) _controlInstructions.OnEnter();
        else _controlInstructions.OnExit();
    }
    public override void OnEnter()
    {
		base.OnEnter();
		_isPause = true;

    }
    public override void OnExit()
    {
		base.OnExit();
		_isPause=false;
    }

    public void OnClickMainMenuButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        //要pop所有的Panel
        UIManager.Instance.PopAllPanel();
        //初始化字典
        UIManager.Instance.InitializePanelDict();
    }
    public void OnClickReturnStartButton()
    { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //要pop所有的Panel
        UIManager.Instance.PopAllPanel();
        //初始化字典
        UIManager.Instance.InitializePanelDict();
        //UIManager.Instance.PushPanel(UIPanelType.SystemSettingPanel);
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
