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


    private bool _animationPlaying, _climbing,  _swingBoost;
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
        transform.parent.parent.transform.position = transform.parent.position;
        transform.parent.parent.transform.rotation = transform.parent.rotation;




        //---------------------------
        // Mantling & Ledge Grabbing
        //---------------------------
        if (Input.GetButtonDown("Jump"))
        {
            if (_wallRunning)
            {
                _wallRunning = false;

                _rigidbody.AddForce(new Vector3(10, 3, 0), ForceMode.Impulse);
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

                if (VaultCast() && !ChestCast())
                {
                    _savedSpeed = _rigidbody.velocity;
                    _animator.enabled = true;
                    _animator.Play("Vault");
                    _animationPlaying = true;

                }
                else if (ChestCast() && HeadCast() && !CapCast())
                {
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
            _savedSpeed = _rigidbody.velocity;
            _animator.enabled = true;
            _animator.Play("Swing");
            _animationPlaying = true;
            _swingBoost = true;
        }
        if (_wallRunning)
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            transform.parent.position += transform.parent.TransformDirection(Vector3.forward) * Time.deltaTime * _savedSpeed.magnitude;

        }
        else
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        }

        Debug.Log(_savedPlayerRotation.eulerAngles.y - 180);


    }

    private void OnTriggerEnter(Collider other)
    {
        //-----------
        // Wall Run
        //-----------

        if(other.tag == "RunableWall")
        {
            _savedSpeed = _rigidbody.velocity;
            _savedPlayerRotation = transform.rotation;
            transform.parent.rotation = Quaternion.Euler(transform.rotation.x, other.transform.rotation.y, transform.rotation.z);
            float numberRanger = Mathf.Round(other.transform.rotation.eulerAngles.y - 360 * Mathf.Floor(other.transform.rotation.eulerAngles.y / 360));
            if(numberRanger == 360)
            {
                numberRanger = 0;
                Debug.Log(numberRanger);
            }
            

            if (_savedPlayerRotation.eulerAngles.y - 180 < numberRanger)
            {
                transform.parent.Rotate(0, 90, 0);
            }
            else
            {
                transform.parent.Rotate(0, -90, 0);

            }
            transform.rotation = _savedPlayerRotation;
            _wallRunning = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "RunableWall")
        {
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

    private bool ChestCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), transform.forward, 1.5f);
    }

    private bool HeadCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, 1);
    }

    private bool CapCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, ClimbCap + 1, 0), transform.forward, 1);

    }



    //------------
    // Coroutines
    //------------
    private IEnumerator StopAnim()
    {
        yield return new WaitForEndOfFrame();
        if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            _animationPlaying = false;
            _animator.enabled = false;
            transform.position = _animationEndPosition;
            transform.parent.position = gameObject.transform.position;
            transform.localPosition = Vector3.zero;
            _rigidbody.AddRelativeForce(_savedSpeed);
            if(_swingBoost)
            {
                _rigidbody.AddForce(new Vector3(0, 3, 0), ForceMode.Impulse);
            }
            _swingBoost = false;

        }
    }
}
