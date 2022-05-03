using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{


    //This script AIMS to contain:

    //Sensing

    //Interaction
    //Climbing
    //Collecting Objects
    //Opening Doors

    //--


    //-----------
    // Variables
    //-----------

    [Tooltip("The Speed General Doors Open At")]
    public float doorOpenSpeed;
    [Tooltip("THe maximum speed the player can move on a ladder")]
    public float climbSpeed;

    private float _rotation, _lookSpeed;

    private float _doorOpenedAmount, _distanceToGround;
    private bool _openDoor, _closeDoor, _climbing;

    private MeshRenderer[] _detectionAreas;
    private PlayerController _playerController;
    private Rigidbody _rigidbody;
    private GameObject _interactText, _targetDoor;
    private Renderer[] _cameras, _interactables;
    private GameObject _playerCamera;


    void Start()
    {
        _interactText = GameObject.Find("InteractText");
        _playerController = GetComponent<PlayerController>();
        _rigidbody = GetComponent<Rigidbody>();
        _distanceToGround = GetComponent<Collider>().bounds.extents.y;
        _lookSpeed = _playerController.lookSpeed;
        //_detectionAreas = GameObject.Find("Detection Areas").GetComponentsInChildren<MeshRenderer>();
        //_cameras = GameObject.Find("Cameras").GetComponentsInChildren<Renderer>();
        _playerCamera = GameObject.Find("PlayerCameraParent");
        //_interactables = GameObject.Find("Interactables").GetComponentsInChildren<Renderer>();


        foreach (var renderer in _detectionAreas)
        {
            renderer.enabled = false;
        }
        foreach (var camera in _cameras)
        {
            camera.material.color = Color.white;
        }
        foreach (var interactable in _interactables)
        {
            interactable.material.color = Color.white;
        }
        Time.timeScale = 1;
    }


    void Update()
    {
        //------------------------
        // Interaction Detection
        //------------------------

        //RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray.origin, ray.direction, out hit, 3) && hit.transform.gameObject.layer == 8)
        //{
        //    //_interactText.SetActive(true);
        //    if (Input.GetButtonDown("Interact"))
        //    {
        //        Interact(hit.collider.tag, hit.collider.gameObject);
        //    }
        //}
        //else
        //{
        //    //_interactText.SetActive(false);
        //}


        //--------------
        // Opening Door
        //--------------

        if (_doorOpenedAmount < 90 && _openDoor == true)
        {
            _targetDoor.transform.Rotate(0, doorOpenSpeed, 0);
            _doorOpenedAmount += doorOpenSpeed;
        }
        else if(_closeDoor == false)
        {
            _doorOpenedAmount = 0;
            _openDoor = false;
        }

        if (_doorOpenedAmount > -90 && _closeDoor == true)
        {
            _targetDoor.transform.Rotate(0, -doorOpenSpeed, 0);
            _doorOpenedAmount -= doorOpenSpeed;
        }
        else if(_openDoor == false)
        {
            _doorOpenedAmount = 0;
            _closeDoor = false;
        }


        //-----------
        // Climbing
        //-----------

        if (_climbing)
        {
            _playerController._mouseY = Input.GetAxis("Mouse Y") * _lookSpeed;

            _rotation += _playerController._mouseY;


            _rigidbody.AddRelativeForce(0, Input.GetAxis("Vertical") / 10, 0, ForceMode.Impulse);
            if (_rigidbody.velocity.magnitude > climbSpeed)
            {
                _rigidbody.velocity = _rigidbody.velocity.normalized * climbSpeed;
            }
            if (Input.GetAxis("Vertical") == 0)
            {
                _rigidbody.velocity = new Vector3(0, 0, 0);
            }

            if ((_rotation > 80 && _playerController._mouseY > 0) || (_rotation < -80 && _playerController._mouseY < 0))
            {
                _rotation -= _playerController._mouseY;
            }
            else
            {
                _playerCamera.transform.Rotate(-_playerController._mouseY, 0, 0);
            }


            //---------------
            //Dismount Check
            //---------------

            Debug.DrawRay(new Vector3(transform.position.x, transform.position.y - _distanceToGround, transform.position.z), transform.forward, Color.red);

            if (Physics.Raycast(transform.position, -Vector3.up, _distanceToGround + 0.1f))
            {
                _climbing = false;
                _playerController.enabled = true;
                _rigidbody.useGravity = true;
                _playerController._mouseY = 0;
                _playerController._playerCamera.transform.rotation = new Quaternion(0, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            }
            else if (!Physics.Raycast(new Vector3(transform.position.x, transform.position.y - _distanceToGround, transform.position.z), transform.forward, 1f))
            {
                transform.position += transform.forward * 0.75f;
                _climbing = false;
                _playerController.enabled = true;
                _rigidbody.useGravity = true;
                _playerController._playerCamera.transform.rotation = new Quaternion(0, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            }
        }

        //----------
        // Sensing
        //----------

        //if (Input.GetButtonDown("Sense"))
        //{
        //    foreach (var renderer in _detectionAreas)
        //    {
        //        renderer.enabled = true;
        //    }
        //    foreach (var camera in _cameras)
        //    {
        //        if(camera.tag == "Destructable")
        //        {
        //            camera.material.color = Color.yellow;

        //        }
        //        else
        //        {
        //            camera.material.color = Color.red;
        //        }
        //    }
        //    foreach (var interactable in _interactables)
        //    {
        //        interactable.material.color = Color.green;
        //    }
        //    Time.timeScale = 0.5f;
        //}
        //else if (Input.GetButtonUp("Sense"))
        //{
        //    foreach (var renderer in _detectionAreas)
        //    {
        //        renderer.enabled = false;
        //    }
        //    foreach (var camera in _cameras)
        //    {
        //        camera.material.color = Color.white;
        //    }
        //    foreach (var interactable in _interactables)
        //    {
        //        interactable.material.color = Color.white;
        //    }
        //    Time.timeScale = 1;
        //}


    }


    private void Interact(string targetTag, GameObject targetObject)
    {
        if (targetTag == "Door")
        {
            _openDoor = true;
            _targetDoor = targetObject;
            _targetDoor.tag = "Opened Door";
        }
        else if (targetTag == "Opened Door")
        {
            _closeDoor = true;
            _targetDoor = targetObject;
            _targetDoor.tag = "Door";
        }
        else if(targetTag == "Key")
        {
            targetObject.SetActive(false);
            //Child a barrier in front of the locked door.
        }
        else if(targetTag == "Climbable")
        {
            _playerController.enabled = false;
            _rigidbody.useGravity = false;
            _rigidbody.velocity = new Vector3(0, 0, 0);
            transform.position = new Vector3(targetObject.transform.position.x , transform.position.y + 1, targetObject.transform.position.z);
            transform.rotation = targetObject.transform.rotation;
            transform.position += transform.forward * -0.5f;
            _climbing = true;
        }
        else
        {
            Debug.LogWarning("Object tag invalid. Check the raycast target tag.");
        }
    }
}
