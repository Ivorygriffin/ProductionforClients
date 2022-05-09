using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parkour : MonoBehaviour
{
    [Header("Vault & Scramble")]
    public float VaultHeight;

    private bool _animationPlaying;
    private Vector3 _animationEndPosition;


    private Animator _animator;
    private IEnumerator _stopAnim;


    void Start()
    {
        _animator = GetComponent<Animator>();

    }


    void Update()
    {
        //----------
        // Mantling
        //----------
        if (Input.GetButtonDown("Jump"))
        {
            if (VaultCast() && !ChestCast())
            {
                _animator.enabled = true;

                _animator.Play("Vault");
                _animationPlaying = true;

            }
            else if (ChestCast() && HeadCast())
            {
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

    private bool VaultCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, VaultHeight - 1, 0), transform.forward, 2f);
    }

    private bool ChestCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), transform.forward, 2f);
    }

    private bool HeadCast()
    {
        return Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, 0.99f);
    }

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
        }
    }
}
