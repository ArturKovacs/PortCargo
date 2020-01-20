using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISmallContainer : IContainerBase, IObjectInteractable
{


    GameObject PickupContainer(GameObject obj);
}
