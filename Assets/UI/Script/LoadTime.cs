using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadTime : MonoBehaviour
{
    public float time=3;
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadingTime());
    }
    private void Update()
    {
        slider.value += Time.deltaTime/3;
    }

    public IEnumerator LoadingTime()
    {
        yield return new WaitForSecondsRealtime(time);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //UIManager.Instance.InitializePanelStack();

    }
}
