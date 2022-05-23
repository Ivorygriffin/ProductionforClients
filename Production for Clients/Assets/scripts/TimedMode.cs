using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimedMode : MonoBehaviour
{
  
    public TMP_Text timerText;

    public void Awake()
    {
        
    }
    void Update()
    {
        TimeData.timeData += Time.deltaTime;
        timerText.text = "Time: " + TimeData.timeData.ToString("F3");
    }

  
}
