using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{



    public AudioClip[] FootstepSounds;
    public AudioClip[] LandingSounds;
    public AudioClip[] SlideSounds;
    public AudioClip VaultSound;
    public AudioClip ClamberSound;
    public AudioClip SwingSound;





    private AudioSource _audioSource;
    private IEnumerator _playFootsteps;
    private bool _stepping, _landing;
    private PlayerController _playerController;
    private int _randomNumber, _previousNumber;
    [HideInInspector]
    public bool _vaulting, _clambering, _swinging;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _playerController = FindObjectOfType<PlayerController>();
    }


    void Update()
    {
        if ((Input.GetButton("Vertical") || Input.GetButton("Horizontal")) && !_stepping && !_playerController._sliding)
        {
            _playFootsteps = Footsteps();
            StartCoroutine(_playFootsteps);
        }
        if (!_playerController._grounded && !_playerController._sliding && _playerController.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1 && _playerController._jumpMargin > .3f)
        {
            _landing = true;
        }
        if(_playerController._grounded && _landing)
        {
            _audioSource.clip = LandingSounds[Random.Range(0, LandingSounds.Length - 1)];
            _audioSource.Play();
            _landing = false;
        }
        if (_playerController._rigidbody.velocity.magnitude > _playerController._savedMaxSpeed + 0.01 && !_playerController._sliding && Input.GetButtonDown("Crouch"))
        {
            _audioSource.clip = SlideSounds[Random.Range(0, SlideSounds.Length - 1)];
            _audioSource.Play();
        }

        if (_vaulting)
        {
            _audioSource.clip = VaultSound;
            _audioSource.Play();
            _vaulting = false;
        }
        if (_clambering)
        {
            _audioSource.clip = ClamberSound;
            _audioSource.Play();
            _clambering = false;
        }
        if (_swinging)
        {
            _audioSource.clip = SwingSound;
            _audioSource.Play();
            _swinging = false;
        }
    }

    private IEnumerator Footsteps()
    {
        if(_playerController._playerSpeed > 0 && !_playerController._sliding && !_audioSource.isPlaying)
        {
            _stepping = true;
            yield return new WaitForSeconds((.2f * 10 / 6) / (_playerController._playerSpeed / _playerController._savedMaxSpeed));
            _randomNumber = Random.Range(0, FootstepSounds.Length - 1);
            if(_randomNumber == _previousNumber && !_playerController._sliding && !_audioSource.isPlaying)
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
            if (!_audioSource.isPlaying)
            {
                _audioSource.clip = FootstepSounds[_randomNumber];
            }

            if (_playerController._grounded && !_playerController._sliding && !_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
            _previousNumber = _randomNumber;
            yield return new WaitForSeconds((.2f * 10 / 6) / (_playerController._playerSpeed / _playerController._savedMaxSpeed));
            _stepping = false;
        }
    }
}