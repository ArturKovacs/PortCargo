using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBox : MonoBehaviour, ISmallContainer
{
    public string containerName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public int inventorySize { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public int weight { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    
    private enum PickupState { Down = 0, Up = 1 };
    private PickupState pickupState { get; set; }

    private Rigidbody _rb;


    public void Awake()
    {
        pickupState = PickupState.Down;
        _rb = GetComponent<Rigidbody>();
    }
    public void CheckContainerInventory()
    {
        throw new System.NotImplementedException();
    }

    public void PickupContainer()
    {
        Debug.Log("PickupContainer from SmallBox script.");
        if (pickupState == PickupState.Down)
        {
            transform.position += new Vector3(0, 2f, 0);
            _rb.isKinematic = true;
            pickupState = PickupState.Up;
        }
        else
        {
            pickupState = PickupState.Down;
            _rb.isKinematic = false;
        }

        
    }
}
