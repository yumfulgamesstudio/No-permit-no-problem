using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class VehicleCamera : MonoBehaviour
{
    //Inputs

    public InputActionAsset playerInputActions;
    private InputAction zoomMouseIA;
    private InputAction zoomGamepadIA;

    //Variables

    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float zoomLeprSpeed = 10f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 15f;

    private CinemachineCamera cam;
    private CinemachineOrbitalFollow orbital;
    private Vector2 scrollDelta;

    private float targetZoom;
    private float currentZoom;

    private void OnEnable()
    {
        
        playerInputActions.FindActionMap("Vehicle").Enable();
        
    }

    private void OnDisable()
    {
        playerInputActions.FindActionMap("Vehicle").Disable();
        
    }

    private void Awake()
    {
        this.enabled = false;
        zoomMouseIA = InputSystem.actions.FindAction("MouseZoom");
        zoomGamepadIA = InputSystem.actions.FindAction("GamepadZoom");
        
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        cam = GetComponent<CinemachineCamera>();
        orbital = cam.GetComponent<CinemachineOrbitalFollow>();

        targetZoom = currentZoom = orbital.Radius;
    }

    public void StartVehicle(GameObject player)
    {

    }



    private void Update()
    {
         HandleMouseScroll();
         HandleGamepadScroll();

        if (scrollDelta.y != 0)
        {
            if(orbital != null)
            {
                targetZoom = Mathf.Clamp(orbital.Radius - scrollDelta.y * zoomSpeed, minDistance, maxDistance);
                scrollDelta = Vector2.zero;
            }
        }

        
        float bumperDelta = zoomGamepadIA.ReadValue<float>();
        if (bumperDelta != 0)
        {
            targetZoom = Mathf.Clamp(orbital.Radius - bumperDelta * zoomSpeed, minDistance, maxDistance);
            
        }

        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomLeprSpeed);
        orbital.Radius = currentZoom;
    }

    private void HandleMouseScroll()
    {
        scrollDelta = zoomMouseIA.ReadValue<Vector2>();
    }

    private void HandleGamepadScroll()
    {
        
    }
}
