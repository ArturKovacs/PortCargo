using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // If velocity is higher, no force will be applied
    [Header("Movement and Pickup settings")]
    public float maxRunSpeed = 10f;
    public float moveForce = 10f;
    public float jumpForce = 2f;
    public float objectInteractDistance = 4f;

    [Header("Camera object")]
    public Transform _cameraTransform;

    private Vector3 _moveDirection;
    private Vector3 _mousePositionWorldSpace;

    private Rigidbody _rb;

    private RaycastHit _cursorPosition;
    private Ray _ray;
    private GameObject _rayCastedObject;

    [SerializeField]
    [CannotBeNullObjectField]
    private GameObject _pickupHolderJoint; 
    
    private GameObject _activeObjectInHand;

    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

    }

    void Update()
    {
        RayCastToCursorPosition();

        GetMovementInputs();
        
        //Debug.Log("Look direction: " + (_mousePositionWorldSpace - this.transform.position).normalized);
    }

    void FixedUpdate()
    {
        MoveCharacter();
        RotateCharacter();
    }

    //
    // Methods for mechanics
    //
    void GetMovementInputs()
    {
        // Horizontal / Vertical should always be local to camera
        //_moveDirection = (Input.GetAxisRaw("Horizontal") * transform.right + Input.GetAxisRaw("Vertical") * transform.forward).normalized;
        //Vector3 cameraForwardLevelled = new Vector3(0f, 0f, _cameraTransform.forward.z);
        //_moveDirection = (Input.GetAxisRaw("Horizontal") * _cameraTransform.right + Input.GetAxisRaw("Vertical") * cameraForwardLevelled).normalized;
        _moveDirection = (Input.GetAxisRaw("Horizontal") * _cameraTransform.right + Input.GetAxisRaw("Vertical") * _cameraTransform.forward).normalized;

        if (Input.GetKeyDown(KeyCode.E))
        {
            InteractWithActiveObject();
        }
    }


    void MoveCharacter()
    {
        // TODO: TUrning on and off force addition causes player character to jitter, need a force decrementor based on speed.
        if(_rb.velocity.magnitude < maxRunSpeed)
            _rb.AddForce(_moveDirection * moveForce);
    }

    void RotateCharacter()
    {
        //if (_lookDirection != Vector3.zero) {
            Vector3 direction = (_mousePositionWorldSpace - this.transform.position).normalized;
            Quaternion rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.deltaTime);
        //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 5f * Time.deltaTime);
            _rb.MoveRotation(rotation);
        //}
    }

    void RayCastToCursorPosition()
    {
        // Get cursor position to determine look direction
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(_ray, out _cursorPosition))
        {
            _mousePositionWorldSpace = _cursorPosition.point;
            Debug.DrawRay(this.transform.position, _mousePositionWorldSpace - this.transform.position);
    }
    }

    private void InteractWithActiveObject()
    {
        if (_activeObjectInHand != null)      // Drop currently held container
        {
            HandleObjectInteraction();
        }
        else                                // If hands empty, use raycasted object and check
        {
            float distanceToObject = Vector3.Distance(_mousePositionWorldSpace, this.transform.position);
            if (_cursorPosition.rigidbody != null && _cursorPosition.rigidbody.gameObject.CompareTag("CargoObjectInteractable"))
            {
                _rayCastedObject = _cursorPosition.rigidbody.gameObject;
                if (distanceToObject <= objectInteractDistance)
                {
                    HandleObjectInteraction();
                }
            }
        }
    }

    void HandleObjectInteraction()
    {
        if (_activeObjectInHand == null)
        {
            _activeObjectInHand = _rayCastedObject.GetComponent<IObjectInteractable>().HandleInteraction(_pickupHolderJoint);
        }
        else
        {
            _activeObjectInHand.GetComponent<IObjectInteractable>().HandleInteraction(_pickupHolderJoint);
            _activeObjectInHand = null;
        }
    }

    public GameObject getActiveObject()
    {
        if (_activeObjectInHand == null)
            return null;
        else
            return _activeObjectInHand;
    }
}
