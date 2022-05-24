using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public string mainMenu;    
    public string forest;    
    public string desert; 
    public string office; 
    public string Endoffice;
    public string prototype;   
    public string targetScene;
    public string targetTimedScene;
    public string cave;

    public GameObject pauseMenu;
    public GameObject levelSelect;

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
        if (Input.GetKey(KeyCode.P))
        {
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0;
            levelSelect.SetActive(true);
            
        }
    }

    public void LoadMain()
    {
        SceneManager.LoadScene(mainMenu);
    }  
    public void LoadForest()
    {
        SceneManager.LoadScene(forest);
    }  
    public void LoadDesert()
    {
        SceneManager.LoadScene(desert);
    } 
    public void LoadOffice()
    {
        SceneManager.LoadScene(office);
        Debug.Log("office loading");
    }  
    public void LoadEndOffice()
    {
        SceneManager.LoadScene(Endoffice);
    }
    public void LoadProtoype()
    {
        SceneManager.LoadScene(prototype);
    }

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
        levelSelect.SetActive(false);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void MouseOn()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void MouseOff()
    {
        Cursor.lockState= CursorLockMode.Confined;
    }


    public void LoadScene()
    {
        LoadingData.sceneToLoad = targetScene;
        SceneManager.LoadScene("LoadingScene");
    } 
    public void LoadTimedScene()
    {
        LoadingData.sceneToLoad = targetTimedScene;
        SceneManager.LoadScene("LoadingScene");
    }

    public void LoadCave()
    {
        SceneManager.LoadScene("cave");
    }

    

}
