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


    private bool _animationPlaying, _climbing, _canSwing, _swingBoost;
    private Vector3 _animationEndPosition, _savedSpeed;


    private Rigidbody _rigidbody;
    private Animator _animator;
    private IEnumerator _stopAnim;


    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
    }


    void Update()
    {
        //---------------------------
        // Mantling & Ledge Grabbing
        //---------------------------
        if (Input.GetButtonDown("Jump"))
        {
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



    }

    private void OnTriggerEnter(Collider other)
    {
        //-----------
        // Swinging
        //-----------

        if (other.tag == "Swingable" && _canSwing)
        {
            _savedSpeed = _rigidbody.velocity;
            _animator.enabled = true;
            _animator.Play("Swing");
            _animationPlaying = true;
            _canSwing = false;
            _swingBoost = true;
        }

        if (other.tag == "SwingCheck")
        {
            _canSwing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "SwingCheck")
        {
            _canSwing = false;
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
                Debug.Log("e");
                _rigidbody.AddForce(new Vector3(0, 20, 0));
            }
            _swingBoost = false;

        }
    }
}
