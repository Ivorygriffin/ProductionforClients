using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    void Update()
    {
        
    }

    public void TimeTrial(bool doTimeTrial)
    {
        if (doTimeTrial)
        {
            LoadingData.isTimed = true;
        }
    }
    public void LoadScene(string SceneToLoad)
    {
        SceneManager.LoadScene(SceneToLoad);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
