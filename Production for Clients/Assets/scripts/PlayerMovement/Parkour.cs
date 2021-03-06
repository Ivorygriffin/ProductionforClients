using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parkour : MonoBehaviour
{
    [Header("Vault & Scramble")]
    [Tooltip("The Height that objects must be to vault over")]
    public float VaultHeight;
    [Tooltip("Height above player head that a ledge grab can be triggered")]
    public float ClimbCap;
    [Tooltip("How mant times further than a far cast that a parkour action can activate")]
    public float ExtendedCastMultiplier;

    [Header("Swing")]
    [Tooltip("How fast the player moves forward when coming out of a swing")]
    public float SwingBoostSpeed;
    [Tooltip("How high the player jumps when coming out of a swing")]
    public float SwingBoostHeight;

    [Header("Other")]
    [Tooltip("How much animations are slowed when the player is moving slow (divides the players velocity by this number)")]
    public float AnimationSpeedMax;
    [Tooltip("How high the player jumps when dismounting from a walljump (% of jump height")]
    public float wallJumpDismountJumpPercent;


    [HideInInspector]
    public bool _wallRunning;
    [HideInInspector]
    public bool canMoveCamera;
    [HideInInspector]
    public bool _animationPlaying;

    private bool _animationStart, _climbing, _farClimb, _midVault, _swingBoost, _approaching;
    private float _animationSpeed, _bumpSpeed;


    //Timers
    private float _MomentumLossTimer;

    private Vector3 _animationEndPosition, _savedSpeed;    
    private Quaternion _savedPlayerRotation;
    private Swing _swingCheck;
    private Rigidbody _rigidbody;
    private Animator _animator;
    private IEnumerator _stopAnim;
    private PlayerController _playerController;
    private PlayerAudio _playerAudio;



    void Start()
    {

        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _swingCheck = GetComponentInChildren<Swing>();
        canMoveCamera = true;
        _playerController = GetComponent<PlayerController>();
        _playerAudio = FindObjectOfType<PlayerAudio>();
    }


    void Update()
    {
        // -----------------------
        // Object Bump Protection
        // -----------------------

        if (_bumpSpeed < _rigidbody.velocity.magnitude)
        {
            _bumpSpeed = _rigidbody.velocity.magnitude;
        }

        if (_rigidbody.velocity.magnitude < _bumpSpeed)
        {
            _MomentumLossTimer += Time.deltaTime;
        }
        else
        {
            _MomentumLossTimer = 0;
        }

        if (_MomentumLossTimer > .25f)
        {
            _bumpSpeed = _rigidbody.velocity.magnitude;
        }






        if (_rigidbody.velocity.magnitude > 1.5f)
        {
            _animationSpeed = _rigidbody.velocity.magnitude / GetComponent<PlayerController>().maxSpeed * AnimationSpeedMax;
        }
        else if(_animationSpeed < AnimationSpeedMax / 2)
        {
            _animationSpeed = AnimationSpeedMax / 2;
        }
        if(_animationSpeed > AnimationSpeedMax)
        {
            _animationSpeed = AnimationSpeedMax;
        }

        if (!_animationPlaying)
        {
            _animator.speed = _animationSpeed;
        }


        //---------------------------
        // Mantling & Ledge Grabbing
        //---------------------------
        if (Input.GetButtonDown("Jump") && !_climbing && !_animationPlaying && !_animationStart && _playerController.groundAngle.x < .1f && _playerController.groundAngle.x > -.1f && _playerController.groundAngle.z < .1f  && _playerController.groundAngle.z > -.1f || _approaching)
        {
            if(_rigidbody.velocity.magnitude < 1)
            {
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x * _bumpSpeed, _rigidbody.velocity.y, _rigidbody.velocity.z * _bumpSpeed);
            }

            if (_wallRunning)
            {
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

                _rigidbody.AddForce(transform.forward * 5 + new Vector3(0, _playerController.jumpForce * (wallJumpDismountJumpPercent / 100), 0), ForceMode.Impulse);
                _savedPlayerRotation = transform.rotation;
                transform.rotation = transform.parent.rotation;
                transform.parent.rotation = _savedPlayerRotation;
            }
            else
            {
                if (ChestCast())
                {
                    _swingCheck.gameObject.SetActive(false);
                }
                else
                {
                    _swingCheck.gameObject.SetActive(true);
                }


                //Check for Vault Hop//

                if (VaultFarCast() && !ChestFarCast() && gameObject.GetComponent<PlayerController>()._grounded)
                {

                    if (!VaultHopFarCast() && !VaultSlideFarCast())
                    {
                        _playerAudio._vaulting = true;
                        _savedSpeed = _rigidbody.velocity;
                        _animator.enabled = true;

                        if (!VaultHopCast() && !VaultSlideCast())
                        {
                            _animator.Play("VaultHop");
                        }
                        else
                        {
                            _animator.Play("VaultHop_Far");
                        }
                        _animationStart = true;
                    }


                    //Check For Vault Slide//

                    else if (!VaultSlideFarCast())
                    {
                        _playerAudio._vaulting = true;
                        _savedSpeed = _rigidbody.velocity;
                        _animator.enabled = true;

                        if (VaultSlideCast())
                        {
                            _animator.Play("VaultSlide");
                        }
                        else
                        {
                            _animator.Play("VaultSlide_Far");
                        }
                        _animationStart = true;
                    }


                    //Play Vault//

                    else
                    {
                        _playerAudio._vaulting = true;
                        _savedSpeed = _rigidbody.velocity;
                        _animator.enabled = true;

                        if (VaultCast())
                        {
                            _animator.Play("Vault");
                        }
                        else
                        {
                            _animator.Play("Vault_Far");
                        }
                        _animationStart = true;
                    }
                }

                //Climb / Clamber//

                else if (ChestFarCast() && HeadFarCast() && !CapFarCast())
                {
                    if(ChestCast() && HeadCast() && !CapCast())
                    {
                        _savedSpeed = _rigidbody.velocity;
                        _rigidbody.velocity = Vector3.zero;
                        _climbing = true;
                    }
                    else
                    {
                        _savedSpeed = _rigidbody.velocity;
                        _rigidbody.velocity = Vector3.zero;
                        _farClimb = true;
                    }
                }
                else if (ChestFarCast() && !HeadFarCast() && VaultFarCast())
                {
                    _midVault = true;
                }
            }
        }

        if (_climbing)
        {
            _playerAudio._clambering = true;
            _rigidbody.AddForce(0, Physics.gravity.magnitude * _playerController.gravityMultiplier / 4, 0, ForceMode.Impulse);
            if (!HeadCast())
            {


                canMoveCamera = false;
                _climbing = false;
                _animator.enabled = true;
                _animator.Play("Climb");
                _animationStart = true;
            }
        }
        if (_farClimb)
        {
            _playerAudio._clambering = true;

            _rigidbody.AddForce(0, Physics.gravity.magnitude * _playerController.gravityMultiplier / 4, 0, ForceMode.Impulse);
            if (!HeadFarCast())
            {
                 

                canMoveCamera = false;

                _farClimb = false;
                _animator.enabled = true;
                _animator.Play("Climb_Far");
                _animationStart = true;
            }
        }
        if (_midVault)
        {
            _playerAudio._clambering = true;

            _rigidbody.AddForce(0, Physics.gravity.magnitude * _playerController.gravityMultiplier / 4, 0, ForceMode.Impulse);
            if (!ChestFarCast())
            {
                 

                if (VaultCast())
                {
                    canMoveCamera = false;

                    _animator.enabled = true;
                    _animator.Play("Vault");
                    _animationStart = true;
                }
                else
                {
                    canMoveCamera = false;

                    _animator.enabled = true;
                    _animator.Play("Vault_Far");
                    _animationStart = true;
                }
                _midVault = false;

            }
        }

        if (_animationStart && !_animationPlaying)
        {
            _stopAnim = StopAnim();
            StartCoroutine(_stopAnim);
            canMoveCamera = false;
            _animationStart = false;

        }

        if (_animationPlaying)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

                _animationPlaying = false;
                _animator.enabled = false;
                transform.position = _animationEndPosition;
                transform.parent.position = gameObject.transform.position;
                transform.localPosition = Vector3.zero;

                if (_swingBoost)
                {
                    _rigidbody.AddRelativeForce(new Vector3(0, SwingBoostHeight, SwingBoostSpeed), ForceMode.Impulse);

                }
                else
                {
                    _playerController._playerSpeed = _savedSpeed.magnitude;
                    _rigidbody.velocity = new Vector3(_savedSpeed.x, 0, _savedSpeed.z);
                }
                _swingBoost = false;
                _swingCheck.gameObject.SetActive(true);
                transform.rotation = new Quaternion(0, 0, 0, 0);
                canMoveCamera = true;
            }
            _animationEndPosition = transform.position;
        }
        else
        {
            transform.parent.position = transform.position;
            transform.localPosition = Vector3.zero;
        }


        //----------------
        // Swing Movement
        //----------------

        if (_swingCheck._startSwing)
        {
            _playerAudio._swinging = true;
            _swingCheck._startSwing = false;
            _swingCheck.gameObject.SetActive(false);
            _savedSpeed = _rigidbody.velocity;
            _animator.enabled = true;
            _animator.Play("Swing");
            _animationStart = true;
            _swingBoost = true;
        }
        if (_wallRunning)
        {

            transform.parent.position += transform.parent.forward * Time.deltaTime * _savedSpeed.magnitude;

        }
        else
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RunableWall")
        {
            _swingCheck.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //-----------
        // Wall Run
        //-----------

        if (other.tag == "RunableWall")
        {

            if (!_wallRunning)
            {

                _savedSpeed = _rigidbody.velocity;

                    _savedPlayerRotation = transform.rotation;
                    transform.parent.rotation = Quaternion.Euler(transform.rotation.x, other.transform.rotation.eulerAngles.y, transform.rotation.z);

                    transform.rotation = _savedPlayerRotation;

                    if (transform.localRotation.w < 0)
                    {
                        transform.rotation = new Quaternion(transform.rotation.x, -transform.rotation.y, transform.rotation.z, -transform.rotation.w);

                    }

                    if (transform.localRotation.y > 0)
                    {
                        transform.parent.Rotate(0, 90, 0);
                    }
                    else
                    {
                        transform.parent.Rotate(0, -90, 0);

                    }
                    transform.rotation = _savedPlayerRotation;
                    _wallRunning = true;
                    _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "RunableWall")
        {
            _swingCheck.gameObject.SetActive(true);
            _wallRunning = false;
            _savedPlayerRotation = transform.rotation;
            transform.rotation = transform.parent.rotation;
            transform.parent.rotation = _savedPlayerRotation;
        }
    }


    //----------
    // Raycasts
    //----------
    private bool VaultCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, VaultHeight - 1.01f, 0), transform.forward, 1.5f);
    }
    private bool VaultFarCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, VaultHeight - 1.01f, 0), transform.forward, 2.5f);
    }
    private bool VaultHopCast()
    {
        return Physics.Raycast(transform.position + transform.forward * 2.5f + new Vector3(0, 0.1f, 0), -transform.up, .5f);
    }

    private bool VaultHopFarCast()
    {
        return Physics.Raycast(transform.position + transform.forward * 3.5f + new Vector3(0, 0.1f, 0), -transform.up, .5f);
    }

    private bool VaultSlideCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), transform.forward, 4f);
    }

    private bool VaultSlideFarCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), transform.forward, 5.5f);
    }

    private bool ChestCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), transform.forward, 2);
    }
    private bool ChestFarCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), transform.forward, 3);
    }

    private bool HeadCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, 1.5f);
    }

    private bool HeadFarCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, 2.5f);
    }

    private bool CapCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, ClimbCap + 1, 0), transform.forward, 1.5f);
    }

    private bool CapFarCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, ClimbCap, 0), transform.forward, 2.5f);
    }



    private bool VaultExtendedCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, VaultHeight - 1.01f, 0), transform.forward, 2.5f * ExtendedCastMultiplier);
    }

    private bool VaultHopExtendedCast()
    {
        return Physics.Raycast(transform.position + transform.forward * 3.5f * ExtendedCastMultiplier + new Vector3(0, 0.1f, 0), -transform.up, .5f);
    }

    private bool VaultSlideExtendedCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), transform.forward, 5.5f * ExtendedCastMultiplier);
    }

    private bool ChestExtendedCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), transform.forward, 3 * ExtendedCastMultiplier);
    }
    private bool HeadExtendedCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, 2.5f * ExtendedCastMultiplier);
    }

    private bool CapExtendedCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, ClimbCap, 0), transform.forward, 2.5f * ExtendedCastMultiplier);
    }


    //------------
    // Coroutines
    //------------
    private IEnumerator StopAnim()
    {
        yield return new WaitForEndOfFrame();
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        _animationPlaying = true;
       
    }
}