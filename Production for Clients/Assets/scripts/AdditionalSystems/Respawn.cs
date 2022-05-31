using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{

    private Vector3 _respawnPoint;
    private IEnumerator _respawn;



    // Start is called before the first frame update
    void Start()
    {
        _respawnPoint = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -100)
        {
            _respawn = RespawnPlayer();
            StartCoroutine(_respawn);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            _respawnPoint = transform.position;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "KillPlane")
        {
            _respawn = RespawnPlayer();
            StartCoroutine(_respawn);
        }
    }

    private IEnumerator RespawnPlayer()
    {
        transform.position = _respawnPoint;
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetComponent<PlayerController>().enabled = false;
        FindObjectOfType<HeadBob>()._animator.enabled = false;
        yield return new WaitForSeconds(1);
        transform.GetComponent<PlayerController>().enabled = true;
        FindObjectOfType<HeadBob>()._animator.enabled = true;


    }
}
