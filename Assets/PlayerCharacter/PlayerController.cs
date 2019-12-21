using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // If velocity is higher, no force will be applied
    public float maxRunSpeed = 10f;

    [SerializeField]
    private float moveForce = 10f;
    private float jumpForce = 2f;

    private Vector3 _moveDirection;
    public Transform _cameraTransform;

    private Vector3 _mousePositionWorldSpace;

    private Rigidbody _rb;
    private RaycastHit _cursorPosition;
    private Ray ray;


    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get cursor position to determine look direction
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out _cursorPosition))
        {
            _mousePositionWorldSpace = _cursorPosition.point;
            Debug.DrawRay(this.transform.position, _mousePositionWorldSpace - this.transform.position);
        }

        // Get movement inputs
        GetMovementInputs();
        RotateCharacter();

        //Debug.Log("Look direction: " + (_mousePositionWorldSpace - this.transform.position).normalized);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Move and rotate character
        MoveCharacter();
    }


    // Methods
    void GetMovementInputs()
    {
        // Horizontal / Vertical should always be local to camera
        _moveDirection = (Input.GetAxisRaw("Horizontal") * transform.right + Input.GetAxisRaw("Vertical") * transform.forward).normalized;
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
}
