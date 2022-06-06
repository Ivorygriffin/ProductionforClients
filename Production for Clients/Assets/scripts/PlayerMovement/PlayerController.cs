using System.Collections;
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


    [Header("Movement")]
    [Tooltip("The maximum speed the player can move")]
    public float maxSpeed;
    [Tooltip("How much the sprint speeds up the player")]
    public float sprintMultiplier;
    [Tooltip("How quickly the player gains speed when running")]
    public float SprintSpeedup;
    [Tooltip("Whether the player has to press Shift to Sprint")]
    public bool AlwaysSprint;

    [Header("Camera")]
    [Tooltip("The speed the camera moves with the mouse (AKA sensitivity)")]
    public float lookSpeed;
    [Tooltip("How strong the FOV change is when sprinting (The higher number, the less it changes)")]
    public float FovChangeDampness;

    [Header("Jump")]
    [Tooltip("Defines the height and vertical speed of the player's jump")]
    public float jumpForce;
    [Tooltip("How strong gravity is")]
    public float gravityMultiplier;

    [Header("Slide")]
    [Tooltip("How much the players maximum speed cap is raised when sliding, affects how fast the player goes when sliding downhill")]
    public float slideBoost;
    [Tooltip("How quickly the player loses momentum while sliding")]
    public float SlideFriction;



    [HideInInspector]
    public float _mouseY;
    [HideInInspector]
    public Vector3 groundAngle;
    [HideInInspector]
    public float _playerSpeed, _jumpMargin;

    private float _mouseX, _rotation, _yForce;

    private bool _crouching, _canJump, _hasJumped;
    private float _slideSlowdown, _crouchDistance, _distanceToGround, _fov;
    private Quaternion _savedPlayerRotation;
    private Animator _animator;
    private Parkour _parkour;

    private ContactPoint[] _contactPoints;
    private Vector3 playerGravity;

    //Timers
    private float _fovEaseIn, _pressJumpTimer;

    [HideInInspector]
    public Rigidbody _rigidbody;
    [HideInInspector]
    public GameObject _playerCamera;
    [HideInInspector]
    public bool _grounded;
    [HideInInspector]
    public bool _sliding;
    [HideInInspector]
    public float _savedMaxSpeed;






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
        _rigidbody.useGravity = false;

    }

    void Update()
    {


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
        else
        {
            _mouseX = 0;
            _mouseY = 0;
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
            Debug.Log("E");
        }
        else
        {
            transform.parent.Rotate(0, _mouseX, 0);

        }

        //--------------------------------------------------------------------------------

        //----------------------
        // Player Speed Limiter
        //----------------------

        if (Input.GetAxis("Vertical") > 0 && !_parkour._animationPlaying)
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
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            if (_sliding)
            {
                _rigidbody.velocity = _rigidbody.velocity.normalized * _playerSpeed;

            }
            else
            {
                if (_rigidbody.velocity.magnitude > maxSpeed)
                {
                    _rigidbody.velocity = _rigidbody.velocity.normalized * maxSpeed;
                    _playerSpeed = maxSpeed;
                }

                if (_rigidbody.velocity.magnitude > _playerSpeed && _rigidbody.velocity.magnitude > _savedMaxSpeed)
                {
                    _rigidbody.velocity = _rigidbody.velocity.normalized * _playerSpeed;
                }

                if (_playerSpeed < _savedMaxSpeed)
                {
                    _playerSpeed = _savedMaxSpeed;
                }
            }

            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _yForce, _rigidbody.velocity.z);
        }





        //------------
        // Crouch
        //------------

        if (Input.GetButtonDown("Crouch") && !_crouching)
        {

            _canJump = false;
            _crouching = true;


            //-------------
            // Slide Check
            //-------------

            if (_rigidbody.velocity.magnitude > _savedMaxSpeed + 0.01 && !_sliding)
            {
                FovChangeDampness = (FovChangeDampness / 10) * 9;

                maxSpeed = _savedMaxSpeed * sprintMultiplier;

                transform.localScale = new Vector3(1, 0.2f, 1);
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.75f, transform.localPosition.z);


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
            FovChangeDampness = (FovChangeDampness * 10) / 9;

        }
        else if (_crouching && !Input.GetButton("Crouch") && !Physics.Raycast(transform.position, Vector3.up, _distanceToGround - 0.1f))
        {
            Stand();
            FovChangeDampness = (FovChangeDampness * 10) / 9;
        }

        ///---------------
        // Slide Movement
        ///---------------
        if (_sliding)
        {
            _slideSlowdown += Time.deltaTime * (SlideFriction + ((-SlideCast() + 1.7f) * 10));

            if (_slideSlowdown > 0)
            {
                maxSpeed = _savedMaxSpeed * sprintMultiplier - _slideSlowdown;

            }
            else
            {
                maxSpeed = _savedMaxSpeed * sprintMultiplier + slideBoost - _slideSlowdown;
            }
            _rigidbody.AddForce(0, -50, 0);
            _playerSpeed += Time.deltaTime * (slideBoost / 3);


            if (_playerSpeed > maxSpeed)
            {
                _playerSpeed = maxSpeed;
            }

            if (_rigidbody.velocity.magnitude <= _savedMaxSpeed / 5)
            {
                _sliding = false;
                _slideSlowdown = 0;

                maxSpeed = _savedMaxSpeed / 5;
                transform.localScale = new Vector3(1, 0.5f, 1);
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.0f, transform.localPosition.z);
                _crouchDistance = 0.5f;
            }
            else if (maxSpeed > _savedMaxSpeed * sprintMultiplier + slideBoost)
            {
                maxSpeed = _savedMaxSpeed * sprintMultiplier + slideBoost;
                _slideSlowdown = 0;
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

        if (_hasJumped)
        {
            _pressJumpTimer += Time.deltaTime;
        }

        if(_pressJumpTimer > .3f)
        {
            _pressJumpTimer = 0;
            _hasJumped = false;
        }

        if (!_grounded)
        {
            _jumpMargin += Time.deltaTime;
        }
        else if (!_hasJumped)
        {
            _jumpMargin = 0;
        }

        if (Input.GetButtonDown("Jump") && _canJump && _jumpMargin < .3f)
        {

            _rigidbody.AddForce(0, jumpForce, 0, ForceMode.Impulse);

        }
        if (Input.GetButtonDown("Jump"))
        {
            _jumpMargin = 1;
            _hasJumped = true;

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
            Camera.main.fieldOfView = _fov + _fovEaseIn;
            maxSpeed = _savedMaxSpeed * sprintMultiplier;

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

        if (!_grounded)
        {
            playerGravity = Physics.gravity * gravityMultiplier;
        }
        else
        {
            if (_grounded)
            {
                playerGravity = -groundAngle * Physics.gravity.magnitude * 10;

            }
            else
            {
                playerGravity = -groundAngle * Physics.gravity.magnitude * gravityMultiplier;

            }
        }

        _rigidbody.AddForce(playerGravity, ForceMode.Force);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag != "Player")
        {
            _grounded = IsGrounded(collision);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _grounded = false;
        groundAngle = Vector3.zero;
    }
    //-----------
    // Functions
    //-----------

    public bool IsGrounded(Collision collision)
    {
        _contactPoints = new ContactPoint[collision.contactCount];
        collision.GetContacts(_contactPoints);
        foreach (var ContactPoint in _contactPoints)
        {
            if (47.5f >= Vector3.Angle(ContactPoint.normal, -Physics.gravity.normalized))
            {
                groundAngle = ContactPoint.normal;
                return true;
            }
        }
        if (Physics.Raycast(transform.position, -transform.up, GetComponent<Collider>().bounds.size.y / 2))
        {
            return true;
        }
        return false;
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
    private float SlideCast()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + transform.forward * .6f + transform.up * 1.5f, -transform.up, out hit, 3.5f);
        return hit.distance;
    }

}
