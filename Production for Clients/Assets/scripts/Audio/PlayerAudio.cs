using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{



    public AudioClip[] FootstepSounds;
    public AudioClip[] LandingSounds;
    public AudioClip[] SlideSounds;
    public AudioClip[] VaultSound;
    public AudioClip[] ClamberSound;
    public AudioClip[] SwingSound;
    public AudioClip WallRunSound;
    public AudioClip RespawnSound;




    private AudioReverbFilter _reverbFilter;
    private AudioSource _audioSource;
    private IEnumerator _playFootsteps;
    private bool _stepping, _landing, _startWallRun;
    private PlayerController _playerController;
    private int _randomNumber, _previousNumber;
    private Parkour _parkour;
    private Respawn _respawn;
    private float _savedVolume;

    [HideInInspector]
    public bool _vaulting, _clambering, _swinging, _playRespawn;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _playerController = FindObjectOfType<PlayerController>();
        _parkour = FindObjectOfType<Parkour>();
        _respawn = FindObjectOfType<Respawn>();
        _savedVolume = _audioSource.volume;
        _reverbFilter = GetComponent<AudioReverbFilter>();
    }


    void Update()
    {
        _reverbFilter.reverbPreset = AudioData.ambienceReverbPreset;
        if ((Input.GetButton("Vertical") || Input.GetButton("Horizontal")) && !_stepping && !_playerController._sliding)
        {
            _playFootsteps = Footsteps();
            StartCoroutine(_playFootsteps);
        }
        if (!_playerController._grounded && !_playerController._sliding && _playerController._jumpMargin > .3f)
        {
            _landing = true;
        }

        if(_playerController.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            _landing = false;
        }
        if (_playerController._grounded && _landing && !_respawn._respawning)
        {
            GetRandom(LandingSounds);

            _audioSource.clip = LandingSounds[_randomNumber];
            _audioSource.Play();
            _landing = false;
        }
        if (_playerController._rigidbody.velocity.magnitude > _playerController._savedMaxSpeed + 0.01 && !_playerController._sliding && Input.GetButtonDown("Crouch"))
        {
            GetRandom(SlideSounds);

            _audioSource.clip = SlideSounds[_randomNumber];
            _audioSource.Play();
        }

        if (_vaulting)
        {
            GetRandom(VaultSound);
            _audioSource.clip = VaultSound[_randomNumber];
            _audioSource.Play();
            _vaulting = false;
        }
        if (_clambering)
        {
            GetRandom(ClamberSound);
            _audioSource.clip = ClamberSound[_randomNumber];
            _audioSource.Play();
            _clambering = false;
        }
        if (_swinging)
        {
            GetRandom(SwingSound);

            _audioSource.clip = SwingSound[_randomNumber];
            _audioSource.Play();
            _swinging = false;
        }
        if (_parkour._wallRunning && _startWallRun)
        {
            _audioSource.clip = WallRunSound;
            _audioSource.Play();
            _startWallRun = false;
        }
        else if (!_parkour._wallRunning)
        {
            if (!_startWallRun)
            {
                _audioSource.Stop();
            }
            _startWallRun = true;
        }
        if (_respawn._respawning && _playRespawn)
        {
            _audioSource.clip = RespawnSound;
            _audioSource.Play();
            _playRespawn = false;
        }
        else if (!_respawn._respawning)
        {
            _playRespawn = true;
        }
        if (_respawn._respawning)
        {
            _audioSource.volume += Time.deltaTime / 1.8f;
        }
        else
        {
            _audioSource.volume = _savedVolume;
        }
    }

    private IEnumerator Footsteps()
    {
        if (_playerController._playerSpeed > 0 && !_playerController._sliding && !_audioSource.isPlaying && !_respawn._respawning)
        {
            _stepping = true;
            yield return new WaitForSeconds((.2f * 10 / 6) / (_playerController._playerSpeed / _playerController._savedMaxSpeed));
            GetRandom(FootstepSounds);

            if (!_audioSource.isPlaying)
            {
                _audioSource.clip = FootstepSounds[_randomNumber];
            }

            if (_playerController._grounded && !_playerController._sliding && !_audioSource.isPlaying)
            {
                _audioSource.Play();
            }

            yield return new WaitForSeconds((.2f * 10 / 6) / (_playerController._playerSpeed / _playerController._savedMaxSpeed));
        }
        _stepping = false;

    }

    private void GetRandom(AudioClip[] audioClips)
    {

        _randomNumber = Random.Range(0, audioClips.Length - 1);
        if (_randomNumber == _previousNumber)
        {

            if (_randomNumber == audioClips.Length - 1)
            {
                _randomNumber = Random.Range(0, audioClips.Length - 2);
            }
            else
            {
                _randomNumber += 1;
            }
        }
        _previousNumber = _randomNumber;

    }
}