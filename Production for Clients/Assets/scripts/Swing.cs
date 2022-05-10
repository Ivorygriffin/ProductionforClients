using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
    public bool _startSwing;
    private void OnTriggerEnter(Collider other)
    {
        //-----------
        // Swinging
        //-----------

        if (other.tag == "Swingable")
        {
            _startSwing = true;

        }

    }

}
