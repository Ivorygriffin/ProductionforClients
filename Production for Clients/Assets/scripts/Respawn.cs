using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{


    private Vector3 _respawn;



    // Start is called before the first frame update
    void Start()
    {
        _respawn = transform.position;

    }

    // Update is called once per frame
    void Update()
    {



        if (transform.position.y < -100)
        {
            transform.position = _respawn;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            _respawn = other.transform.position;
            Debug.Log("A");
        }
        if (other.tag == "KillPlane")
        {
            transform.position = _respawn;
            Debug.Log("B");
        }
    }
}
