using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBreathing : MonoBehaviour
{
    public float MaxVolume;

    private AudioSource _audioSource;
    private PlayerController _playerController;


    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Vertical") >= 1)
        {
            _audioSource.volume += Time.deltaTime / 7;
        }
        else
        {
            _audioSource.volume -= Time.deltaTime / 7;
        }

        if(_audioSource.volume > MaxVolume)
        {
            _audioSource.volume = MaxVolume;
        }
    }
}
