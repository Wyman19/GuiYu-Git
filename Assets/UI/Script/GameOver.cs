using UnityEngine;

public class GameOver : BasePanel
{
    public void OnClickReturnButton()
    {
        UIManager.Instance.PopPanel();
    }
}
