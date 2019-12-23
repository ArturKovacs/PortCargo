﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // If velocity is higher, no force will be applied
    public float maxRunSpeed = 10f;

    [SerializeField]
    private float moveForce = 10f;
    private float jumpForce = 2f;
    private float pickupDistance = 2f;

    private Vector3 _moveDirection;
    public Transform _cameraTransform;

    private Vector3 _mousePositionWorldSpace;

    private Rigidbody _rb;
    private RaycastHit _cursorPosition;
    private Ray ray;
    private GameObject rayCastedObject;


    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

        RayCastToCursorPosition();

        // Get movement inputs
        GetMovementInputs();
        RotateCharacter();

        //Debug.Log("Look direction: " + (_mousePositionWorldSpace - this.transform.position).normalized);
    }

    void FixedUpdate()
    {
        // Move character
        MoveCharacter();
    }

    //
    // Methods
    //
    void GetMovementInputs()
    {
        // Horizontal / Vertical should always be local to camera
        //_moveDirection = (Input.GetAxisRaw("Horizontal") * transform.right + Input.GetAxisRaw("Vertical") * transform.forward).normalized;
        Vector3 cameraForwardLevelled = new Vector3(0f, 0f, _cameraTransform.forward.z);
        _moveDirection = (Input.GetAxisRaw("Horizontal") * _cameraTransform.right + Input.GetAxisRaw("Vertical") * cameraForwardLevelled).normalized;
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
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 5f * Time.deltaTime);
        //}
    }

    void RayCastToCursorPosition()
    {
        // Get cursor position to determine look direction
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out _cursorPosition))
        {
            _mousePositionWorldSpace = _cursorPosition.point;
            Debug.DrawRay(this.transform.position, _mousePositionWorldSpace - this.transform.position);

            float distanceToObject = Vector3.Distance(_mousePositionWorldSpace, this.transform.position);

            if (_cursorPosition.rigidbody != null && _cursorPosition.rigidbody.gameObject.CompareTag("CargoObject"))
            {
                rayCastedObject = _cursorPosition.rigidbody.gameObject;
                //Debug.Log("That's a box!");

                if(Input.GetKeyDown(KeyCode.E) && distanceToObject <= pickupDistance)
                {
                    Debug.Log("TODO: Pickup Object");
                    rayCastedObject.GetComponent<ISmallContainer>().PickupContainer();
                }
            }
            
        }
    }
}
