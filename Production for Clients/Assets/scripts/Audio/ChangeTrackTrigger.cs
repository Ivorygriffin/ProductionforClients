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
    [Tooltip("How fast the transition between tracks occurs (in seconds)")]
    public float transitionSpeed;
    [Tooltip("The BPM of the song (Required for onlyOnBeat")]
    public float BPM;




    private AudioSource[] _audioSource;
    private bool _fadeDown, _fadeUp, _queueChange;
    private IEnumerator _fadeClip;
    private Music _music;
    private int _activeAudioSource, _otherAudioSource;

    private void Start()
    {
        _music = FindObjectOfType<Music>();
        _activeAudioSource = 0;
        _otherAudioSource = 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _audioSource = GameObject.Find("MusicSource").gameObject.GetComponents<AudioSource>();
            AudioData.queueChange = true;
        }

    }
    private void Update()
    {
        if (_fadeDown)
        {
            _audioSource[_activeAudioSource].volume -= Time.deltaTime / transitionSpeed;
        }
        if (_fadeUp)
        {
            _audioSource[_otherAudioSource].volume += Time.deltaTime / transitionSpeed;
            if(_audioSource[_otherAudioSource].volume == 1)
            {
                if (_activeAudioSource == 1)
                {
                    _activeAudioSource = 0;
                    _otherAudioSource = 1;
                }
                else
                {
                    _activeAudioSource = 1;
                    _otherAudioSource = 0;
                }
            }
        }
        if (AudioData.queueChange && _music.canChangeTrack)
        {
            if (fade)
            {
                _fadeClip = FadeClip();
                StartCoroutine(_fadeClip);
            }
            else
            {
                _audioSource[_activeAudioSource].clip = trackToChangeTo;
                _audioSource[_activeAudioSource].Play();
            }
            AudioData.queueChange = false;
        }
    }

    public IEnumerator FadeClip()
    {
        _fadeDown = true;
        _audioSource[_otherAudioSource].clip = trackToChangeTo;
        _audioSource[_otherAudioSource].Play();
        _fadeUp = true;


        yield return new WaitForSeconds(transitionSpeed);
        _fadeDown = false;
        _fadeUp = false;
    }
}
