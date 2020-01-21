using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [Header("Target")]
    [SerializeField]
    private Transform _defaultFollowTarget;
    private Transform _activeFollowTarget;
    public float followSpeed = 3f;

    private Transform parentTransform;

    [Header("Camera Sensitivity and limits")]
    public float cameraSensitivity_y = 8f;
    public float cameraSensitivity_x = 2f;
    public float maxRotX = 55f;
    public float minRotX = 45f;

    [Header("Camera Offset to Target settings")]
    public Vector3 defaultPosition;
    public Vector3 defaultRotation;

    public enum CursorLockState { Unlocked = 0, Locked = 1 };
    public CursorLockState cursorLockState = CursorLockState.Unlocked;

   void Awake()
    {
        if (_defaultFollowTarget == null)
        {
            ThrowTargetNotSetError();
        }
        else 
        {
            parentTransform = this.transform.parent.transform;
            parentTransform.position = _defaultFollowTarget.position;
        } 

    }

    private void Start()
    {
        //transform.position = defaultPosition;
        setNewOffsetPosition(defaultPosition);
        transform.rotation = Quaternion.Euler(defaultRotation);

        _activeFollowTarget = _defaultFollowTarget;
    }


    void LateUpdate()
    {
        if (_activeFollowTarget != null)
        {
            // Move the camera to the (maybe interpolated) position. 
            TrackTarget();

            //TODO: Fix Zoom going away
            //ZoomCamera();

            //Rotate camera
            if (Input.GetMouseButton(2))
            {
                RotateCamera();
            }


        }
        else
            ThrowTargetNotSetError();
    }

    void ThrowTargetNotSetError()
    {
        Debug.LogError("FollowTarget must be initialized for the main camera! Attach an object's transform.");
    }



    void RotateCamera()
    {
        // Y ROTATION
        Vector3 oldAngles = parentTransform.localEulerAngles;
        float rotationY = oldAngles.y;
        rotationY += Input.GetAxis("Mouse X") * cameraSensitivity_y;

        parentTransform.localEulerAngles = new Vector3(oldAngles.x, rotationY, oldAngles.z);

        // X ROTATION
        Vector3 oldAnglesCam = transform.localEulerAngles;
        float rotationX = oldAnglesCam.x;
        rotationX += Input.GetAxis("Mouse Y") * cameraSensitivity_x;
        rotationX = Mathf.Clamp(rotationX, minRotX, maxRotX);

        transform.localEulerAngles = new Vector3(rotationX, oldAnglesCam.y, oldAnglesCam.z);
    }

    void ZoomCamera()
    {
        //if(Input.GetAxis("Mouse ScrollWheel") < 0)
        //{
        //    transform.position -= transform.forward;
        //    defaultPosition = transform.position;
        //}
        //else if(Input.GetAxis("Mouse ScrollWheel") > 0)
        //{
        //    transform.position += transform.forward;
        //    defaultPosition = transform.position;
        //}        
    }

    void TrackTarget()
    {
        parentTransform.position = Vector3.Lerp(parentTransform.position, _activeFollowTarget.transform.position, Time.deltaTime * followSpeed);
    }

    void toggleCursorState()
    {
        if(cursorLockState == CursorLockState.Unlocked)
        {
            cursorLockState = CursorLockState.Locked;
            Cursor.visible = false;
        }
        else
        {
            cursorLockState = CursorLockState.Unlocked;
            Cursor.visible = true;
        }
    }

    public void setTrackingTarget(GameObject newTarget)
    {
        _activeFollowTarget = newTarget.transform;
    }

    public void setTrackingTarget(GameObject newTarget, Vector3 newOffset)
    {
        _activeFollowTarget = newTarget.transform;
        setNewOffsetPosition(newOffset);
    }

    private void setNewOffsetPosition(Vector3 newTransformPos)
    {
        this.gameObject.transform.localPosition = newTransformPos;
        //gameObject.GetComponent<Transform>().position = newTransformPos;
        //Camera.main.transform.localPosition = newTransformPos;
    }

    private void resetDefaultOffsetPosition()
    {
        setNewOffsetPosition(defaultPosition);
    }

    public void resetDefaultTrackingTarget()
    {
        _activeFollowTarget = _defaultFollowTarget;
        resetDefaultOffsetPosition();
    }
}
