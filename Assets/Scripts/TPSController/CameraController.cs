using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform followTarget;

    [SerializeField] float rotationSpeed = 2;
    [SerializeField] float distance = 5;
    [SerializeField] float minVerticalAngle = -45;
    [SerializeField] float maxVerticalAngle = 45;
    [SerializeField] Vector2 framingOffset;
    [SerializeField] bool invertX;
    [SerializeField] bool invertY;

    float invertXval;
    float invertYval;

    float rotationX;
    float rotationY;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void InputChange()
    {
        invertXval = (invertX) ? -1 : 1;
        invertYval = (invertY) ? -1 : 1;

        rotationX += Input.GetAxis("Camera Y") * invertXval * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);
        rotationY += Input.GetAxis("Camera X") * invertYval * rotationSpeed;

    }

    void Update()
    {
        InputChange();

        var targetRotation = Quaternion.Euler(rotationX,rotationY, 0);
        var focusPosition = followTarget.position  +new Vector3(framingOffset.x, framingOffset.y);
        transform.position = focusPosition- targetRotation * new Vector3(0,0, distance);
        transform.rotation = targetRotation;
    }

    public Quaternion PlaneRotation => Quaternion.Euler(0,rotationY,0);
}
