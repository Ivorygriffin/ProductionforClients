using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Respawn : MonoBehaviour
{

    private Vector3 _respawnPoint;
    private IEnumerator _respawn, _beatReset;
    private Image _DeathFade;
    private float _deathFadeFade;
    private bool _fadingOut;

    [HideInInspector]
    public bool _respawning;



    // Start is called before the first frame update
    void Start()
    {
        _respawnPoint = transform.position;
        _DeathFade = GameObject.Find("DeathFade").GetComponent<Image>();
        _DeathFade.color = new Color(_DeathFade.color.r, _DeathFade.color.g, _DeathFade.color.b, 0f);
        _DeathFade.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -100)
        {
            _respawn = RespawnPlayer();
            StartCoroutine(_respawn);
        }

        if (_fadingOut)
        {
            _deathFadeFade -= Time.deltaTime * 1.5f;
            _DeathFade.color = new Color(_DeathFade.color.r, _DeathFade.color.g, _DeathFade.color.b, _deathFadeFade);
        }
        if (_respawning)
        {
            transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

            transform.GetComponent<Rigidbody>().AddForce(0, -Physics.gravity.magnitude / 4, 0);
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
        _respawning = true;
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetComponent<PlayerController>().enabled = false;
        FindObjectOfType<HeadBob>()._animator.enabled = false;
        yield return new WaitForSeconds(1);
        _deathFadeFade = 1f;

        FindObjectOfType<Music>()._lowPassFrequency = 500;
        FindObjectOfType<Music>().GetComponents<AudioSource>()[AudioData.activeAudioSource].Stop();
        FindObjectOfType<Music>().GetComponents<AudioSource>()[AudioData.activeAudioSource].clip = AudioData.activeToLoop;
        FindObjectOfType<Music>()._respawned = true;
        FindObjectOfType<Music>().GetComponents<AudioSource>()[AudioData.activeAudioSource].Play();

        ;
        _DeathFade.enabled = true;

        _DeathFade.color = new Color(_DeathFade.color.r, _DeathFade.color.g, _DeathFade.color.b, 1f);
        transform.position = _respawnPoint;
        _fadingOut = true;
        yield return new WaitForSeconds(1);
        _fadingOut = false;
        transform.GetComponent<PlayerController>().enabled = true;
        FindObjectOfType<HeadBob>()._animator.enabled = true;
        _respawning = false;
        _DeathFade.enabled = false;

    }
}
