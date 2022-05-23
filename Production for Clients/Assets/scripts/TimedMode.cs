using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimedMode : MonoBehaviour
{
    private float currentTime;
    public TMP_Text timerText;

    void Update()
    {
        currentTime += Time.deltaTime;
        timerText.SetText("Time: " + currentTime.ToString("00"));
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
