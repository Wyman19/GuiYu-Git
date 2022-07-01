using UnityEngine;

public class Setting : BasePanel
{
    public void OnClickReturnButton()
    {
        UIManager.Instance.PopPanel();
    }
}
