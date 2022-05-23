using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerBox : MonoBehaviour
{
    public UnityEvent onTrigger;
    public UnityEvent onExit;
   
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            onTrigger.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            onExit.Invoke();
        }
    }


}
