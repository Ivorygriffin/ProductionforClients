using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimedMode : MonoBehaviour
{
  
    public TMP_Text timerText;

    public void Awake()
    {
        if (LoadingData.isTimed)
        {
            timerText.gameObject.SetActive(true);
        }
        else
        {
            timerText.gameObject.SetActive(false);
        }
    }
    void Update()
    {
        if (LoadingData.isTimed)
        {
            TimeData.timeData += Time.deltaTime;
            timerText.text = "Time: " + TimeData.timeData.ToString("F3");
        }

    }

  
}
