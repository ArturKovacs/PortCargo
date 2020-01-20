using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IContainerBase
{
    string containerName { get; set; }

    int inventorySize { get; set; }
    int weight { get; set; }

    void CheckContainerInventory();
}
