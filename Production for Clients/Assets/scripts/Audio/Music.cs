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





    private PlayerController _playerController;
    private Parkour _parkour;
    private Rigidbody _rigidbody;

    private AudioSource _audioSource;
    private AudioLowPassFilter _lowPassFilter;
    private AudioHighPassFilter _highPassFilter;


    void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _parkour = FindObjectOfType<Parkour>();
        _rigidbody = FindObjectOfType<PlayerController>().GetComponent<Rigidbody>();

        _audioSource = GetComponent<AudioSource>();
        _lowPassFilter = GetComponent<AudioLowPassFilter>();
        _highPassFilter = GetComponent<AudioHighPassFilter>();

        _lowPassFilter.enabled = false;
        _highPassFilter.enabled = false;
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
            else if(_lowPassFilter.cutoffFrequency > LowPassMin)
            {
                _lowPassFilter.cutoffFrequency -= Time.deltaTime * TransitionSpeed;
            }
        }
        else
        {
            _lowPassFilter.enabled = false;
            _highPassFilter.enabled= true;
            if(_playerController._playerSpeed > _playerController._savedMaxSpeed * _playerController.sprintMultiplier)
            {
                _highPassFilter.cutoffFrequency += Time.deltaTime * 300;
            }
            else
            {
                _highPassFilter.cutoffFrequency -= Time.deltaTime * 2500;

            }

        }
    }
}
