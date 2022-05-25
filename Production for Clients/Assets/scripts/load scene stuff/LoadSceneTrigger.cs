using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneTrigger : MonoBehaviour
{
    [SerializeField] Image progressBar;

    private IEnumerator _loadScene;
    private bool hasRun;

    private void Start()
    {
        hasRun = false;
    }
    void Update()
    {
        if (hasRun == false)
        {
            _loadScene = LoadSceneAsyc();
            StartCoroutine(_loadScene);
            
        }
     
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
                hasRun = true;
            }
            yield return null;
        }
    }
   
   
}
