using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class triggerBox : MonoBehaviour
{
    public UnityEvent toDo;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            toDo.Invoke();
        }
    }
}
