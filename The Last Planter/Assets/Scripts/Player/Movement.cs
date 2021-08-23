using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform camParent;
    [SerializeField] [Range(0f, 2f)] private float sensitivity;
    [SerializeField] [Range(0f, 1f)] private float cameraSmoothness;
    [SerializeField] private float minCamRotation;
    [SerializeField] private float maxCamRotation;

    [Header("Movement Settings")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] [Range(0f, 100f)] private float movementSpeed;

    private float rawMouseDx = 0f;
    private float rawMouseDy = 0f;
    private float rawCamRotationX = 0f;

    private float rawMoveX = 0f;
    private float rawMoveZ = 0f;
    private float rawPlayerRotationZ = 0f;

    Touch touch = new Touch();
    private void Update()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
            DesktopInput();
        else if (SystemInfo.deviceType == DeviceType.Handheld)
            MobileInput();
    }
    private void LateUpdate()
    {        
        CamParentRotation();
        CamRotation();
    }
    private void FixedUpdate()
    {
        PlayerMovement();
    }
    private void DesktopInput()
    {
        rawMouseDy += Input.GetAxis("Mouse Y") * sensitivity;
        rawMouseDx += Input.GetAxis("Mouse X") * sensitivity;
        rawMoveX += Input.GetAxisRaw("Horizontal");
        rawMoveZ += Input.GetAxisRaw("Vertical");
    }
    private void MobileInput()
    {
        if (touch.position.x > 0.5f)
        {
            rawMouseDy = touch.deltaPosition.y * sensitivity;
            rawMouseDx = touch.deltaPosition.x * sensitivity;
        }
        else
        {
            rawMoveX = touch.deltaPosition.y * sensitivity;
            rawMoveZ = touch.deltaPosition.x * sensitivity;
        }
    }
    private void PlayerMovement()
    {
        float divider = (rawMoveX != 0f && rawMoveZ != 0f) ? 1.414f : 1f;
        Vector3 dir = camParent.transform.right * rawMoveX + camParent.transform.forward * rawMoveZ;

        rb.AddForce(dir * movementSpeed / divider, ForceMode.Force);
       

        rawMoveX = rawMoveZ = 0f;
    }
    private void CamParentRotation()
    {
        rawPlayerRotationZ += rawMouseDx;
        rawMouseDx = 0f;

        float currentRotation = camParent.transform.eulerAngles.y;

        float rotationDifference = Mathf.Abs(rawPlayerRotationZ - currentRotation);
        if (rotationDifference > 180f)
        {
            float correction = (int)((rotationDifference - 180) / 360) + 360;
            correction *= Mathf.Sign(rawPlayerRotationZ - currentRotation) * -1f;
            rawPlayerRotationZ += correction;
        }
        currentRotation = Mathf.Lerp(currentRotation, rawPlayerRotationZ, cameraSmoothness);

        camParent.transform.eulerAngles = Vector3.up * currentRotation;
    }
    private void CamRotation()
    {
        rawCamRotationX += rawMouseDy;
        rawMouseDy = 0f;
        rawCamRotationX = Mathf.Clamp(rawCamRotationX, minCamRotation, maxCamRotation);

        float currentRotation = cam.transform.localEulerAngles.x;
        if (currentRotation > -minCamRotation + 1f)
            currentRotation = 360f - currentRotation;
        else
            currentRotation *= -1f;
        currentRotation = Mathf.Lerp(currentRotation, rawCamRotationX, cameraSmoothness);

        cam.transform.localEulerAngles = Vector3.left * currentRotation;
    }
}
