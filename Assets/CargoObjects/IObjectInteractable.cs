using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectInteractable
{
    GameObject HandleInteraction(GameObject caller);
}
