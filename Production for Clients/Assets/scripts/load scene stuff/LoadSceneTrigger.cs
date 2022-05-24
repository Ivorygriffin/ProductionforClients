using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneTrigger : MonoBehaviour
{
    [SerializeField] Image progressBar;

    void Start()
    {
        StartCoroutine(LoadSceneAsyc());
    }

    IEnumerator LoadSceneAsyc()
    {
        yield return new WaitForSeconds(1);
        AsyncOperation operation = SceneManager.LoadSceneAsync(LoadingData.sceneToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            progressBar.fillAmount = operation.progress;
            if(operation.progress >= 0.9f)
            {
                operation.allowSceneActivation=true;
            }
            yield return null;
        }
    }
   
   
}
