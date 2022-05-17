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

    public GameObject pauseMenu;

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            pauseMenu.SetActive(true);
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
    }  
    public void LoadEndOffice()
    {
        SceneManager.LoadScene(Endoffice);
    }

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.None;
        pauseMenu.SetActive(false);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }









}
