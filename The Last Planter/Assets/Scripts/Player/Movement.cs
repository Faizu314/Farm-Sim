using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] private Camera cam;
    [SerializeField] [Range(0f, 2f)] private float sensitivity;
    [SerializeField] [Range(0f, 1f)] private float cameraSmoothness;
    [SerializeField] private float minCamRotation;
    [SerializeField] private float maxCamRotation;

    [Header("Movement Settings")]
    [SerializeField] [Range(0f, 1f)] private float movementSpeed;

    private float rawMouseDz = 0f;
    private float rawMouseDy = 0f;
    private float rawCamRotationX = 0f;

    private float rawMoveX = 0f;
    private float rawMoveZ = 0f;
    private Vector3 moveDirection;
    private float rawPlayerRotationZ = 0f;

    Touch touch = new Touch();
    private void Update()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
            DesktopInput();
        else if (SystemInfo.deviceType == DeviceType.Handheld)
            MobileInput();
        PlayerMovement();
        CamMovement();
    }

    private void DesktopInput()
    {
        rawMouseDy = Input.GetAxis("Mouse Y") * sensitivity;
        rawMouseDz = Input.GetAxis("Mouse X") * sensitivity;
        rawMoveX = Input.GetAxis("Horizontal") * movementSpeed;
        rawMoveZ = Input.GetAxis("Vertical") * movementSpeed;
    }
    private void MobileInput()
    {
        if (touch.position.x > 0.5f)
        {
            rawMouseDy = touch.deltaPosition.y * sensitivity;
            rawMouseDz = touch.deltaPosition.x * sensitivity;
        }
        else
        {
            rawMoveX = touch.deltaPosition.y * sensitivity;
            rawMoveZ = touch.deltaPosition.x * sensitivity;
        }
    }

    private void PlayerMovement()
    {
        moveDirection.x = rawMoveX;
        moveDirection.z = rawMoveZ;
        if (rawMoveX == 0f && rawMoveZ == 0f)
            moveDirection /= 1.414f;
        transform.Translate(moveDirection);

        transform.Rotate(Vector3.up * rawMouseDz);
    }
    private void CamMovement()
    {
        rawCamRotationX += rawMouseDy;
        rawCamRotationX = Mathf.Clamp(rawCamRotationX, minCamRotation, maxCamRotation);

        float currentRotation = cam.transform.eulerAngles.x;
        if (currentRotation > -minCamRotation + 1f)
            currentRotation = 360f - currentRotation;
        else
            currentRotation *= -1f;
        currentRotation = Mathf.Lerp(currentRotation, rawCamRotationX, cameraSmoothness);

        cam.transform.localEulerAngles = new Vector3(-currentRotation, 0, 0);
    }
}
