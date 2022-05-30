using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
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
        Debug.Log(_playerController._savedMaxSpeed * .25f);
        if (_rigidbody.velocity.magnitude < _playerController.maxSpeed * _playerController.sprintMultiplier)
        {
            _highPassFilter.enabled = false;
            _lowPassFilter.enabled = true;

            if (_playerController.maxSpeed != _playerController._savedMaxSpeed)
            {
                _lowPassFilter.cutoffFrequency = _playerController._playerSpeed / _playerController.maxSpeed * 15000 - _playerController._savedMaxSpeed * .5f;
            }
            else
            {
                _lowPassFilter.cutoffFrequency = _playerController._savedMaxSpeed * .5f * 1000;
            }
        }
        else
        {
            _lowPassFilter.enabled = false;
            _highPassFilter.enabled= true;
            _highPassFilter.cutoffFrequency = _playerController._playerSpeed / (_playerController._savedMaxSpeed * _playerController.sprintMultiplier);

        }
    }
}
