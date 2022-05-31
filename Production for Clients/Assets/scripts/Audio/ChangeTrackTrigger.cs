using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTrackTrigger : MonoBehaviour
{
    public AudioClip trackToChangeTo;
    [Tooltip("If true, the audio fades to silence, then fades back in to the new track")]
    public bool fade;

    [Header("Transition Timing")]
    [Tooltip("If true, the audio can only change tracks at the start of a bar")]
    public bool onlyOnBeat;
    [Tooltip("The BPM of the song (Required for onlyOnBeat")]
    public float tempo;




    private AudioSource _audioSource;
    private bool _fadeDown, _fadeUp, _queueChange;
    private IEnumerator _changeClip;
    private Music _music;


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

        if (_queueChange && _music.canChangeTrack)
        {

        }
    }

    public IEnumerator ChangeClip()
    {
        if (fade)
        {
            yield return new WaitForSeconds(1);

            _fadeDown = false;
            _audioSource.clip = trackToChangeTo;
            _audioSource.Play();
            _fadeUp = true;


            yield return new WaitForSeconds(1);
            _fadeUp = false;
        }

        _queueChange = true;

    }
}
