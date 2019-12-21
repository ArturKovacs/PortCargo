using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISmallContainer : IContainerBase
{
    string containerName { get; set; }

    void PickupContainer();
}
