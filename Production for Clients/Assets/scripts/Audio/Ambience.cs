using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambience : MonoBehaviour
{
    private AudioSource _audioSource;
    private AudioReverbFilter _reverbFilter;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _reverbFilter = GetComponent<AudioReverbFilter>();
        AudioData.currentAmbience = _audioSource.clip;
        AudioData.ambienceReverbPreset = _reverbFilter.reverbPreset;
    }

    // Update is called once per frame
    void Update()
    {
        _audioSource.clip = AudioData.currentAmbience;
        _reverbFilter.reverbPreset = AudioData.ambienceReverbPreset;
        if (!_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
    }
}
