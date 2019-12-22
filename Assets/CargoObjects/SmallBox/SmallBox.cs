using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBox : MonoBehaviour, ISmallContainer
{
    public string containerName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public int inventorySize { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public int weight { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void CheckContainerInventory()
    {
        throw new System.NotImplementedException();
    }

    public void PickupContainer()
    {
        Debug.Log("PickupContainer from SmallBox script.");
    }
}
