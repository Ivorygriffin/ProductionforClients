using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    [Tooltip("The minimum value the high pass filter will reach")]
    public float LowPassMin;
    [Tooltip("The maximum value the high pass filter will reach")]
    public float LowPassMax;
    [Tooltip("How quickly the Low Pass Filter Cuttoff frequency changes")]
    public float TransitionSpeed;
    [Tooltip("How much faster the Transition Speed is when lowering, compared to when incresing")]
    public float TransitionNegativeMultiplier;
    [Tooltip("The BPM of the starting song")]
    public float BPM;



    [HideInInspector]
    public bool canChangeTrack;
    [HideInInspector]
    public IEnumerator _beatCounter;


    private PlayerController _playerController;
    private Parkour _parkour;
    private Rigidbody _rigidbody;

    private AudioSource[] _audioSources;
    private AudioLowPassFilter _lowPassFilter;
    private AudioHighPassFilter _highPassFilter;


    private float _savedBPM;
    private bool _firstRun, _changeGate;


    void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _parkour = FindObjectOfType<Parkour>();
        _rigidbody = FindObjectOfType<PlayerController>().GetComponent<Rigidbody>();

        _audioSources = GetComponents<AudioSource>();
        _lowPassFilter = GetComponent<AudioLowPassFilter>();
        _highPassFilter = GetComponent<AudioHighPassFilter>();
        AudioData.currentBPM = BPM;
        _savedBPM = AudioData.currentBPM;
        AudioData.activeAudioSource = 0;
        AudioData.otherAudioSource = 1;

        _audioSources[AudioData.otherAudioSource].enabled = false;
        _audioSources[AudioData.activeAudioSource].Stop();

        _firstRun = true;
        _lowPassFilter.enabled = false;
        _highPassFilter.enabled = false;
        canChangeTrack = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_playerController._playerSpeed < _playerController._savedMaxSpeed * _playerController.sprintMultiplier + .1f && _highPassFilter.cutoffFrequency < 50)
        {
            _highPassFilter.enabled = false;
            _lowPassFilter.enabled = true;

            if (Input.GetButton("Vertical") && Input.GetAxis("Vertical") > 0 && _lowPassFilter.cutoffFrequency < LowPassMax)
            {
                _lowPassFilter.cutoffFrequency += Time.deltaTime * TransitionSpeed;
            }
            else if (_lowPassFilter.cutoffFrequency > LowPassMin)
            {
                _lowPassFilter.cutoffFrequency -= Time.deltaTime * (TransitionSpeed * TransitionNegativeMultiplier);
            }
        }
        else
        {
            _lowPassFilter.enabled = false;
            _highPassFilter.enabled = true;
            if (_playerController._playerSpeed > _playerController._savedMaxSpeed * _playerController.sprintMultiplier)
            {
                _highPassFilter.cutoffFrequency += Time.deltaTime * 700;
            }
            else
            {
                _highPassFilter.cutoffFrequency -= Time.deltaTime * 4000;

            }

        }

    }

    private void FixedUpdate()
    {
        if (canChangeTrack)
        {

            _beatCounter = BeatCounter();
            canChangeTrack = false;
            StartCoroutine(_beatCounter);
            if (_firstRun)
            {
                _audioSources[AudioData.activeAudioSource].Play();
                _firstRun = false;
            }
        }
        if (_savedBPM != AudioData.currentBPM && !_changeGate)
        {
            _beatCounter = BeatCounter();
            StartCoroutine(_beatCounter);
            _changeGate = true;
        }
    }

    public IEnumerator BeatCounter()
    {
        yield return new WaitForSecondsRealtime(60 / AudioData.currentBPM * 4 - 0.04f);
        yield return new WaitForFixedUpdate();
        if (_savedBPM == AudioData.currentBPM)
        {
            canChangeTrack = true;
            Debug.Log("Tick");
        }
        else
        {
            _savedBPM = AudioData.currentBPM;
            _changeGate = false;

        }
    }
}