using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class SceneChangeTrigger : MonoBehaviour
{

    public string targetScene;


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Time.timeScale = 1;
            LoadingData.sceneToLoad = targetScene;
            SceneManager.LoadScene("LoadingScene");
        }
    }


}
