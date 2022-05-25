using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public string mainMenu;    
    public string forest; 
    public string forestTime;    
    public string desert; 
    public string desertTime; 
    public string office; 
    public string officeTime; 
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
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenu);
    }  
    public void LoadForest()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(forest);
    } 
    public void LoadForestTime()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(forestTime);
    }  
    public void LoadDesert()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(desert);
    }    
    public void LoadDesertTime()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(desertTime);
    } 
    public void LoadOffice()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(office);
        Debug.Log("office loading");
    }  
    public void LoadOfficeTime()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(officeTime);
        Debug.Log("office loading");
    }  
    public void LoadEndOffice()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(Endoffice);
    }
    public void LoadProtoype()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(prototype);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
        levelSelect.SetActive(false);
     
    }

    public void Restart()
    {
        Time.timeScale = 1;
        
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
        Time.timeScale = 1;
        SceneManager.LoadScene("cave");
    }

    

}
