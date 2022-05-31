using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    private PlayerController _playerController;
    private Parkour _parkour;
    [HideInInspector]
    public Animator _animator;


    void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_playerController._playerSpeed < 1 || _playerController._sliding || !_playerController._grounded)
        {
            _animator.speed = 0;
        }
        else
        {
            _animator.speed = _playerController._playerSpeed / _playerController._savedMaxSpeed;
        }
    }
}
