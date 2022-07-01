using UnityEngine;

public class Scene_1Root : MonoBehaviour
{
    void Start()
    {
        UIManager.Instance.PushPanel(UIPanelType.PlayerState);
    }
}

