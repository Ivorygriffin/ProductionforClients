using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTrackTrigger : MonoBehaviour
{
    public AudioClip trackToChangeTo;

    private AudioSource _audioSource;
    private bool _fadeDown, _fadeUp;
    private IEnumerator _changeClip;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _audioSource = GameObject.Find("MusicSource").gameObject.GetComponent<AudioSource>();
            _changeClip = ChangeClip();
            StartCoroutine(_changeClip);

            _fadeDown = true;
        }

    }
    private void Update()
    {
        if (_fadeDown)
        {
            _audioSource.volume -= Time.deltaTime;
        }
        if (_fadeUp)
        {
            _audioSource.volume += Time.deltaTime;
        }
    }

    public IEnumerator ChangeClip()
    {
        yield return new WaitForSeconds(1);

        _fadeDown = false;
        _audioSource.clip = trackToChangeTo;
        _audioSource.Play();
        _fadeUp = true;


        yield return new WaitForSeconds(1);
        _fadeUp = false;

    }
}
