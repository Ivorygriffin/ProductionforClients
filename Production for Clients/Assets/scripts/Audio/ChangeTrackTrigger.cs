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

    private void Start()
    {
        _music = FindObjectOfType<Music>();


        _audioSource[AudioData.otherAudioSource].volume = 0;
        _audioSource[AudioData.otherAudioSource].enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _audioSource = GameObject.Find("MusicSource").gameObject.GetComponents<AudioSource>();
            _queueChange = true;
        }

    }
    private void Update()
    {
        if (_fadeDown)
        {
            _audioSource[AudioData.activeAudioSource].volume -= Time.deltaTime / transitionSpeed;
        }
        if (_fadeUp)
        {
            _audioSource[AudioData.otherAudioSource].volume += Time.deltaTime / transitionSpeed;
            if (_audioSource[AudioData.otherAudioSource].volume == 1)
            {
                if (AudioData.activeAudioSource == 1)
                {
                    AudioData.activeAudioSource = 0;
                    AudioData.otherAudioSource = 1;
                }
                else
                {
                    AudioData.activeAudioSource = 1;
                    AudioData.otherAudioSource = 0;
                }
                _audioSource[AudioData.otherAudioSource].enabled = false;

            }
        }
        if (onlyOnBeat)
        {
            if (_queueChange && _music.canChangeTrack)
            {
                AudioData.currentBPM = BPM;
                if (fade)
                {
                    _fadeClip = FadeClip();
                    StartCoroutine(_fadeClip);
                }
                else
                {
                    _audioSource[AudioData.activeAudioSource].clip = trackToChangeTo;
                    _audioSource[AudioData.activeAudioSource].Play();
                }
                _queueChange = false;
            }
        }
        else if (_queueChange && !onlyOnBeat)
        {
            AudioData.currentBPM = BPM;
            if (fade)
            {
                _fadeClip = FadeClip();
                StartCoroutine(_fadeClip);
            }
            else
            {
                _audioSource[AudioData.activeAudioSource].clip = trackToChangeTo;
                _audioSource[AudioData.activeAudioSource].Play();
            }
            _queueChange = false;
        }
    }

    public IEnumerator FadeClip()
    {
        _audioSource[AudioData.otherAudioSource].enabled = true;

        _fadeDown = true;
        _audioSource[AudioData.otherAudioSource].clip = trackToChangeTo;
        _audioSource[AudioData.otherAudioSource].Play();
        _fadeUp = true;


        yield return new WaitForSeconds(transitionSpeed);
        _fadeDown = false;
        _fadeUp = false;
    }
}
