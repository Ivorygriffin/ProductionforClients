using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

    public AudioMixer audioMixer;


    [HideInInspector]
    public bool canChangeTrack, _respawned;
    [HideInInspector]
    public IEnumerator _beatCounter;
    [HideInInspector]
    public float _lowPassFrequency, _songLength;


    private PlayerController _playerController;
    private Parkour _parkour;
    private Rigidbody _rigidbody;

    private AudioSource[] _audioSources;

    public AudioClip _audioLoopToPlay;

    private float _savedBPM, _playTime, _highPassFrequency;
    private bool _firstRun, _changeGate;


    void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _parkour = FindObjectOfType<Parkour>();
        _rigidbody = FindObjectOfType<PlayerController>().GetComponent<Rigidbody>();
        _audioSources = GetComponents<AudioSource>();
        AudioData.currentBPM = BPM;
        _savedBPM = AudioData.currentBPM;
        AudioData.activeAudioSource = 0;
        AudioData.otherAudioSource = 1;

        AudioData.activeToLoop = _audioLoopToPlay;

        _audioSources[AudioData.otherAudioSource].volume = 0;
        _audioSources[AudioData.otherAudioSource].Stop();
        _audioSources[AudioData.activeAudioSource].Stop();

        _audioSources[AudioData.activeAudioSource].loop = false;
        _audioSources[AudioData.otherAudioSource].loop = true;

        _audioSources[AudioData.otherAudioSource].clip = AudioData.activeToLoop;
        _audioSources[AudioData.otherAudioSource].enabled = false;

        _lowPassFrequency = 500;
        _highPassFrequency = 0;

        _firstRun = true;
        canChangeTrack = true;

    }

    // Update is called once per frame
    void Update()
    {
        audioMixer.SetFloat("LowPassFreq", _lowPassFrequency);
        audioMixer.SetFloat("HighPassFreq", _highPassFrequency);

        if (_playerController._playerSpeed < _playerController._savedMaxSpeed * _playerController.sprintMultiplier + .1f && _highPassFrequency < 50)
        {


            if (Input.GetButton("Vertical") && Input.GetAxis("Vertical") > 0 && _lowPassFrequency < LowPassMax)
            {              
                _lowPassFrequency += Time.deltaTime * TransitionSpeed;
            }
            else if (_lowPassFrequency > LowPassMin)
            {
                _lowPassFrequency -= Time.deltaTime * (TransitionSpeed * TransitionNegativeMultiplier);
            }
        }
        else
        {
            if (_playerController._playerSpeed > _playerController._savedMaxSpeed * _playerController.sprintMultiplier)
            {
                _highPassFrequency += Time.deltaTime * 700;
            }
            else
            {
                _highPassFrequency -= Time.deltaTime * 4000;

            }

        }
        if (_playTime >= _songLength - 1.5f)
        {
                _playTime = 0;
                _songLength = Mathf.Infinity;
            _audioSources[AudioData.activeAudioSource].volume = 0;
            _audioSources[AudioData.otherAudioSource].volume = 1;
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
            _audioSources[AudioData.otherAudioSource].enabled = false;

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
                _songLength = _audioSources[0].clip.length;
                _audioSources[AudioData.otherAudioSource].enabled = true;
                _audioSources[AudioData.activeAudioSource].Play();
                _audioSources[AudioData.otherAudioSource].Play();

            }
        }
        if (_savedBPM != AudioData.currentBPM && !_changeGate)
        {
            _beatCounter = BeatCounter();
            StartCoroutine(_beatCounter);
            _changeGate = true;
        }

        _playTime += Time.fixedDeltaTime;
        if (_respawned)
        {
            StopCoroutine(_beatCounter);
            StartCoroutine(_beatCounter);
            _respawned = false;
        }
    }

    public IEnumerator BeatCounter()
    {

        if (_firstRun)
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForSecondsRealtime(60 / AudioData.currentBPM * 4 - 0.04f);

            _firstRun = false;

        }
        else
        {
            yield return new WaitForFixedUpdate();
            canChangeTrack = false;
            yield return new WaitForSecondsRealtime(60 / AudioData.currentBPM * 4 - 0.04f);
        }
        Debug.Log(_savedBPM);
        Debug.Log(AudioData.currentBPM);

        if (_savedBPM == AudioData.currentBPM)
        {
            Debug.Log("Tick");

            canChangeTrack = true;
        }
        else
        {
            _savedBPM = AudioData.currentBPM;
            _changeGate = false;

        }
    }
}