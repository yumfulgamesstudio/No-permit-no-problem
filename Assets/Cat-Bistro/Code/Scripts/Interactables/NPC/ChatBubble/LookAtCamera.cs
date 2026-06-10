using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public enum Method
    {
        LookAt,
        LookAtInverted,
        CameraForward,
        CameraForwardInverted
    }

    [SerializeField] private Method method;

    private Transform mainCameraTransform;

    private void Awake()
    {
        FindMainCamera();
    }

    private void Update()
    {
        if (mainCameraTransform == null)
        {
            FindMainCamera();
        }

        if (mainCameraTransform == null)
            return;

        LookAt();
    }

    private void OnEnable()
    {
        if (mainCameraTransform == null)
        {
            FindMainCamera();
        }

        if (mainCameraTransform != null)
        {
            LookAt();
        }
    }

    private void FindMainCamera()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            mainCameraTransform = mainCamera.transform;
        }
    }

    private void LookAt()
    {
        switch (method)
        {
            default:
            case Method.LookAt:
                transform.LookAt(mainCameraTransform.position);
                break;

            case Method.LookAtInverted:
                Vector3 dir = (transform.position - mainCameraTransform.position).normalized;
                transform.LookAt(transform.position + dir);
                break;

            case Method.CameraForward:
                transform.forward = mainCameraTransform.forward;
                break;

            case Method.CameraForwardInverted:
                transform.forward = -mainCameraTransform.forward;
                break;
        }
    }
}