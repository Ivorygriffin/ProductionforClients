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

    [Header("Player Speed")]
    public float SwingBoostSpeed;
    [Tooltip("How much animations are slowed when the player is moving slow (divides the players velocity by this number)")]
    public float AnimationSpeedDivider;

    private bool _animationPlaying, _climbing, _swingBoost;
    private Vector3 _animationEndPosition, _savedSpeed;

    private Quaternion _savedPlayerRotation;
    private Swing _swingCheck;
    private Rigidbody _rigidbody;
    private Animator _animator;
    private IEnumerator _stopAnim;
    [HideInInspector]
    public bool _wallRunning;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _swingCheck = GetComponentInChildren<Swing>();
    }


    void Update()
    {


        //---------------------------
        // Mantling & Ledge Grabbing
        //---------------------------
        if (Input.GetButtonDown("Jump") && !_climbing && !_animationPlaying)
        {

            if (_wallRunning)
            {
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

                _rigidbody.AddForce(transform.forward * 5 + new Vector3(0, 4, 0), ForceMode.Impulse);
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

                if (VaultFarCast() && !ChestFarCast() && gameObject.GetComponent<PlayerController>().IsGrounded())
                {

                    if (!VaultHopFarCast() && !VaultSlideFarCast())
                    {
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

                        _animator.speed = _rigidbody.velocity.magnitude / 5 + 0.5f;
                        _animationPlaying = true;

                    }
                    else if (!VaultSlideFarCast())
                    {
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

                        _animator.speed = _rigidbody.velocity.magnitude / 5 + 0.5f;
                        _animationPlaying = true;

                    }                
                    else
                    {
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

                        _animator.speed = _rigidbody.velocity.magnitude / 5 + 0.5f;
                        _animationPlaying = true;

                }

            }
                else if (ChestCast() && HeadCast() && !CapCast())
                {
                    _animator.speed = _rigidbody.velocity.magnitude / 5 + 0.5f;
                    _climbing = true;

                }

            }

        }

        if (_climbing)
        {
            _rigidbody.AddForce(0, 30, 0);
            if (!HeadCast())
            {
                _savedSpeed = _rigidbody.velocity;
                _climbing = false;
                _animator.enabled = true;
                _animator.Play("Climb");
                _animationPlaying = true;
            }
        }

        if (_animationPlaying)
        {
            _animationEndPosition = transform.position;
            _stopAnim = StopAnim();
            StartCoroutine(_stopAnim);

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
            _swingCheck._startSwing = false;
            _swingCheck.gameObject.SetActive(false);
            _savedSpeed = _rigidbody.velocity;
            _animator.enabled = true;
            _animator.speed = _rigidbody.velocity.magnitude / 5 + 0.5f;
            _animator.Play("Swing");
            _animationPlaying = true;
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
                    Debug.Log("a");
                    Debug.Log(transform.localRotation.w);
                    Debug.Log(transform.localRotation.y);

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

    private bool CapCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, ClimbCap + 1, 0), transform.forward, 1.5f);

    }



    //------------
    // Coroutines
    //------------
    private IEnumerator StopAnim()
    {
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForEndOfFrame();
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
                _rigidbody.AddRelativeForce(new Vector3(0, SwingBoostSpeed / 2, SwingBoostSpeed), ForceMode.Impulse);
            }
            else
            {
                _rigidbody.AddRelativeForce(new Vector3(_savedSpeed.x, 0, _savedSpeed.z));

            }
            _swingBoost = false;
            _swingCheck.gameObject.SetActive(true);
            transform.rotation = new Quaternion(0, 0, 0, 0);


        }
    }
}
