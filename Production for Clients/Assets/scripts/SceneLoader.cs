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

    public GameObject pauseMenu;
    public GameObject levelSelect;

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.Confined;
            pauseMenu.SetActive(true);
        }
        if (Input.GetKey(KeyCode.P))
        {
            levelSelect.SetActive(true);
            Cursor.lockState= CursorLockMode.Confined;
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
