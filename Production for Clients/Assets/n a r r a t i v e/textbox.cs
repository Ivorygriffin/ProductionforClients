using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class textbox : MonoBehaviour
{
    public string[] texts;
    public TMP_Text dialogue;
    public int currentArrayNum;
    public GameObject dialogueBox;

    public void Start()
    {
        currentArrayNum = 0;
        dialogue.text = texts[currentArrayNum];
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            currentArrayNum += 1;
           dialogue.text = texts[currentArrayNum];
        }
        if(currentArrayNum == texts.Length)
        {
            dialogueBox.SetActive(false);
        }
    }
}
