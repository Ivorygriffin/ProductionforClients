using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioClip[] FootstepSounds;
    public AudioClip[] LandingSounds;


    private AudioSource _audioSource;
    private IEnumerator _playFootsteps;
    private bool _stepping, _landing;
    private PlayerController _playerController;
    private int _randomNumber, _previousNumber;


    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _playerController = FindObjectOfType<PlayerController>();
    }


    void Update()
    {
        if ((Input.GetButton("Vertical") || Input.GetButton("Horizontal")) && !_stepping)
        {
            _playFootsteps = Footsteps();
            StartCoroutine(_playFootsteps);
        }
        if (!_playerController._grounded && !_playerController._sliding && _playerController.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1 && _playerController.groundAngle.magnitude < 20)
        {
            _landing = true;
        }
        if(_playerController._grounded && _landing)
        {
            _audioSource.clip = LandingSounds[Random.Range(0, LandingSounds.Length - 1)];
            _audioSource.Play();
            _landing = false;
        }
        if (_playerController._sliding)
        {
            //play slide audio
            //change based on speed (volume?)
        }
    }

    private IEnumerator Footsteps()
    {
        if(_playerController._playerSpeed > 0)
        {
            _stepping = true;
            yield return new WaitForSeconds((.2f * 10 / 6) / (_playerController._playerSpeed / _playerController._savedMaxSpeed));
            _randomNumber = Random.Range(0, FootstepSounds.Length - 1);
            if(_randomNumber == _previousNumber)
            {
                if(_randomNumber == FootstepSounds.Length - 1)
                {
                    _randomNumber = Random.Range(0, FootstepSounds.Length - 2);
                }
                else
                {
                    _randomNumber += 1;
                }
            }
            _audioSource.clip = FootstepSounds[_randomNumber];

            if (_playerController._grounded && !_playerController._sliding)
            {
                _audioSource.Play();
            }
            _previousNumber = _randomNumber;
            yield return new WaitForSeconds((.2f * 10 / 6) / (_playerController._playerSpeed / _playerController._savedMaxSpeed));
            _stepping = false;
        }
    }
}
