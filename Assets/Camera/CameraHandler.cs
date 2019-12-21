using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public Transform FollowTarget;
    public float followSpeed = 3f;

    public Vector3 defaultPosition;
    public Vector3 defaultRotation;

   void Start()
    {
        this.transform.position = defaultPosition;
        this.transform.rotation = Quaternion.Euler(defaultRotation);
    }
    void LateUpdate()
    {
        if (FollowTarget != null)
        {
            // Move the camera to the (maybe interpolated) position. 
            transform.position = Vector3.Lerp(transform.position, FollowTarget.transform.position + defaultPosition, Time.deltaTime * followSpeed);
        }
    }
}
