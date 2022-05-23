﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour
{
    //------------------------------

    //This script contains:

    //Player Movement
    //Sprinting

    //Jumping

    //Crouching
    //Sliding

    //Vaulting

    //Mouse Lock

    //------------------------------    



    //------------
    // Variables
    //------------

    [Tooltip("Whether the player has to press Shift to Sprint")]
    public bool AlwaysSprint;
    [Header("Movement Values")]
    [Tooltip("The maximum speed the player can move")]
    public float maxSpeed;
    [Tooltip("How much the sprint speeds up the player")]
    public float sprintMultiplier;
    [Tooltip("The speed the camera moves with the mouse (AKA sensitivity)")]
    public float lookSpeed;
    [Tooltip("How long before the player can roll again after rolling (Seconds)")]
    public float rollCooldownLength;
    [Tooltip("Defines the height and vertical speed of the player's jump")]
    public float jumpForce;
    [Tooltip("How quickly the player loses momentum while sliding")]
    public float SlideFriction;
    [Tooltip("How quickly the player gains speed when running")]
    public float SprintSpeedup;
    [Tooltip("How strong the FOV change is when sprinting (The higher number, the less it changes)")]
    public float FovChangeDampness;



    [HideInInspector]
    public float _mouseY;


    private float _mouseX, _rotation, _yForce, _savedMaxSpeed;

    private bool _sliding, _crouching, _canJump;
    private float _slideSlowdown, _crouchDistance, _distanceToGround, _fov, _playerSpeed;
    private Quaternion _savedPlayerRotation;
    private Animator _animator;
    private Parkour _parkour;

    //Timers
    private float _fovEaseIn;

    private Rigidbody _rigidbody;
    [HideInInspector]
    public GameObject _playerCamera;






    void Start()
    {
        //--------------------------------
        // Finding Components and Objects
        //--------------------------------
        _rigidbody = GetComponent<Rigidbody>();
        _playerCamera = GameObject.Find("PlayerCameraParent");
        _savedMaxSpeed = maxSpeed;
        _distanceToGround = GetComponent<Collider>().bounds.extents.y;
        Cursor.lockState = CursorLockMode.Locked;
        _fov = Camera.main.fieldOfView;
        _canJump = true;
        _animator = GetComponent<Animator>();
        _animator.enabled = false;
        _parkour = GetComponent<Parkour>();
    }

    void Update()
    {


        if (IsGrounded())
        {
            _rigidbody.useGravity = false;
            if (!Input.GetButton("Jump"))
            {
                _rigidbody.AddForce(new Vector3(0, -5, 0));
            }
            if (!Input.GetButton("Vertical") && !Input.GetButton("Horizontal"))
            {
                _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }

        }
        else
        {
            _rigidbody.useGravity = true;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }


        _savedPlayerRotation.y = _rigidbody.transform.rotation.y;
        _savedPlayerRotation.w = _rigidbody.transform.rotation.w;

        //-----------------
        // Camera Movement
        //-----------------
        if (_parkour.canMoveCamera)
        {
            _mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            _mouseY = Input.GetAxis("Mouse Y") * lookSpeed;
        }


        _rotation += _mouseY;

        if ((_rotation > 80 && _mouseY > 0) || (_rotation < -80 && _mouseY < 0))
        {
            _rotation -= _mouseY;
        }
        else
        {
            _playerCamera.transform.Rotate(-_mouseY, 0, 0);
        }
        if (_parkour._wallRunning)
        {
            transform.Rotate(0, _mouseX, 0);

        }
        else
        {
            transform.parent.Rotate(0, _mouseX, 0);

        }

        //--------------------------------------------------------------------------------



        //----------------------
        // Player Speed Limiter
        //----------------------

        if (Input.GetAxis("Vertical") > 0)
        {
            _playerSpeed += Time.deltaTime * SprintSpeedup;
        }
        else if (Input.GetAxis("Vertical") < 0 || Input.GetButton("Horizontal"))
        {
            _playerSpeed = maxSpeed;
        }
        else
        {
            _playerSpeed = 0;
        }

        _yForce = _rigidbody.velocity.y;

        if (_rigidbody.velocity.magnitude > maxSpeed)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * maxSpeed;
        }
        if (_rigidbody.velocity.magnitude > _playerSpeed)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * SprintSpeedup;

        }
        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _yForce, _rigidbody.velocity.z);




        //------------
        // Crouch
        //------------

        if (Input.GetButtonDown("Crouch") && !_crouching)
        {

            _canJump = false;
            _crouching = true;


            //--------
            // Slide
            //--------

            if (_rigidbody.velocity.magnitude > _savedMaxSpeed + 0.01 && _sliding == false)
            {
                maxSpeed = _savedMaxSpeed * sprintMultiplier;
                transform.localScale = new Vector3(1, 0.2f, 1);
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.2f, transform.localPosition.z);
                _crouchDistance = 0.15f;
                _sliding = true;

            }
            else if (_rigidbody.velocity.magnitude <= _savedMaxSpeed + 0.01)
            {
                maxSpeed = _savedMaxSpeed / 5;
                transform.localScale = new Vector3(1, 0.5f, 1);
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.5f, transform.localPosition.z);
                _crouchDistance = 0.5f;
            }
        }



        if (Input.GetButtonUp("Crouch") && !Physics.Raycast(transform.position, Vector3.up, _distanceToGround - 0.1f))
        {
            Stand();
        }
        else if (_crouching && !Input.GetButton("Crouch") && !Physics.Raycast(transform.position, Vector3.up, _distanceToGround - 0.1f))
        {
            Stand();
        }

        if (_sliding)
        {
            maxSpeed = _savedMaxSpeed * sprintMultiplier - _slideSlowdown + 1f;
            _slideSlowdown += Time.deltaTime * SlideFriction;

            if (_rigidbody.velocity.magnitude <= _savedMaxSpeed / 5)
            {
                _sliding = false;
                _slideSlowdown = 0;

                maxSpeed = _savedMaxSpeed / 5;
                transform.localScale = new Vector3(1, 0.5f, 1);
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.0f, transform.localPosition.z);
                _crouchDistance = 0.5f;
            }
        }
        else if (!_sliding && _crouching)
        {
            maxSpeed = _savedMaxSpeed / 5;
            transform.localScale = new Vector3(1, 0.5f, 1);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.0f, transform.localPosition.z);
            _crouchDistance = 0.5f;
        }



        //----------
        // Jumping
        //----------

        if (Input.GetButtonDown("Jump") && _canJump && IsGrounded())
        {

            _rigidbody.AddForce(0, jumpForce, 0, ForceMode.Impulse);

        }

        if (!Input.GetButton("Horizontal") && !Input.GetButton("Vertical"))
        {
            _rigidbody.velocity = new Vector3(0, _yForce, 0);
        }


        if (Input.GetButtonDown("Horizontal"))
        {
            if (!Input.GetButton("Vertical"))
            {
                _rigidbody.velocity = new Vector3(0, _yForce, 0);
            }
        }
        if (Input.GetButtonDown("Vertical"))
        {
            if (!Input.GetButton("Horizontal"))
            {
                _rigidbody.velocity = new Vector3(0, _yForce, 0);
            }
        }

    }


    private void FixedUpdate()
    {
        //----------------
        // Player Movement
        //----------------
        _rigidbody.AddRelativeForce(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"), ForceMode.Impulse);


        //--------
        // Sprint
        //--------

        if (Input.GetAxis("Vertical") > 0 & AlwaysSprint == true)
        {
            maxSpeed = _savedMaxSpeed * sprintMultiplier;
            Camera.main.fieldOfView = _fov + _fovEaseIn;

        }
        else
        {
            maxSpeed = _savedMaxSpeed;
            Camera.main.fieldOfView = _fov + _fovEaseIn;
            if (_fovEaseIn > 0)
            {
                _fovEaseIn -= Time.deltaTime * _fovEaseIn;
            }
        }

        if (_playerSpeed > 0 && Input.GetAxis("Vertical") > 0)
        {
            _fovEaseIn = _playerSpeed / FovChangeDampness;
        }


    }



    //-----------
    // Functions
    //-----------

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, _distanceToGround + 0.1f);
    }


    private void Stand()
    {
        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + _crouchDistance, transform.localPosition.z);
        _crouching = false;
        _canJump = true;
        _sliding = false;
        _slideSlowdown = 0;
    }


}
