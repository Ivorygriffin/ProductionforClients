using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    private GameObject pauseMenu;
    private GameObject levelSelect;

    private Parkour _parkour;
    private PlayerController _playerController;

    private void Start()
    {
        pauseMenu = GameObject.Find("Mini Menu");
        levelSelect = GameObject.Find("Level Select");
        if(pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
        if(levelSelect != null)
        {
            levelSelect.SetActive(false);
        }
        _parkour = FindObjectOfType<Parkour>();
        _playerController = FindObjectOfType<PlayerController>();
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && pauseMenu != null)
        {
            Cursor.lockState = CursorLockMode.None;
            _parkour.enabled = false;
            _playerController.enabled = false;
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
        if (Input.GetKey(KeyCode.P) && levelSelect != null)
        {
            Cursor.lockState = CursorLockMode.None;
            _parkour.enabled = false;
            _playerController.enabled = false;
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

        SceneManager.LoadScene("scenes/LoadingScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
    public void Resume()
    {
        Time.timeScale = 1;
        _parkour.enabled = true;
        _playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        if(pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        if (levelSelect != null)
        {
            levelSelect.SetActive(false);
        }

    }

    public void Restart()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
