using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    private GameObject pauseMenu;
    private GameObject levelSelect;

    private void Start()
    {
        pauseMenu = GameObject.Find("Mini Menu");
        levelSelect = GameObject.Find("Level Select");
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && pauseMenu != null)
        {
            Cursor.lockState = CursorLockMode.None;
            FindObjectOfType<Parkour>().canMoveCamera = false;
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
        if (Input.GetKey(KeyCode.P) && levelSelect != null)
        {
            Cursor.lockState = CursorLockMode.None;
            FindObjectOfType<Parkour>().canMoveCamera = false;
            Time.timeScale = 0;
            levelSelect.SetActive(true);

        }
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
        LoadingData.sceneToLoad = SceneToLoad;

        SceneManager.LoadScene("LoadingScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
    public void Resume()
    {
        Time.timeScale = 1;
        FindObjectOfType<Parkour>().canMoveCamera = true;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
        levelSelect.SetActive(false);

    }
}
