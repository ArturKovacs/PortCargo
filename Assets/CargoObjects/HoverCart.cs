using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverCart : MonoBehaviour, IDriveable
{
    private Rigidbody _rb;

    [SerializeField]
    private float floatingHeight = 0.2f;
    private float currentHeight = 0f;
    [SerializeField]
    private float floatForceMultiplier = 9.81f;

    [SerializeField]
    private float cartMass = 40f;

    public bool isInteracting { get; private set;}

    [CannotBeNullObjectField]
    private Transform _cameraInteractPivotPoint;

    // RayCasts
    private RaycastHit _cursorPosition;
    private Ray ray;
    private Transform groundPoint;

    // Camera Offset while interacting
    public Vector3 cameraOffsetInteracting;

    void Start()
    {
        
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.angularDrag = 0.5f;
        _rb.mass = cartMass;
        _rb.drag = 1f;

        floatForceMultiplier *= cartMass;
    }

    // Update is called once per frame
    void Update()
    {
        currentHeight = GetCurrentHeightByRayCast();
    }

    void FixedUpdate()
    {
        HandleFloating();
    }


    void HandleFloating()
    {
        //float distanceToFloatingHeightMultiplier = Vector3.Distance(transform.position, groundPoint.position);
        float distanceToFloatingHeightMultiplier = 1;
        if (currentHeight < floatingHeight)
        {
            _rb.AddForce(Vector3.up * floatForceMultiplier * distanceToFloatingHeightMultiplier);
        }
    }

    float GetCurrentHeightByRayCast()
    {
        float calculatedHeight = 0;

        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
        {
            groundPoint = hitInfo.transform;
            return hitInfo.distance;
        }
        return 0;
    }

    public GameObject HandleInteraction(GameObject caller)
    {
        if (!isInteracting)
        {
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            if(cam != null)
            {
                cam.GetComponent<CameraHandler>().setTrackingTarget(this.gameObject, cameraOffsetInteracting);
                isInteracting = true;
            }
            
        }
        else
        {
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            if (caller.GetComponentInParent<PlayerController>().getActiveObject() == this.gameObject)
            {
                cam.GetComponent<CameraHandler>().resetDefaultTrackingTarget();
                isInteracting = false;
            }
        }

        return this.gameObject;
    }
}
