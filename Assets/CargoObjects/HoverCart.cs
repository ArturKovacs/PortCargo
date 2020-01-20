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

    // RayCasts
    private RaycastHit _cursorPosition;
    private Ray ray;
    private Transform groundPoint;

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
        throw new System.NotImplementedException();
    }
}
