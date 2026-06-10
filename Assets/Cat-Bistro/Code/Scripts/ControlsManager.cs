using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class ControlsManager : MonoBehaviour
{
    public static ControlsManager instance;

    public InputSystem_Actions controls { get; private set; }

    [Header("References")]
    [SerializeField] private Player player;

    [Header("Cameras")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject carCamera;

    private Car_Interact currentCar;
    private bool isInCar;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        if (PlayerInputManager.Instance == null)
        {
            Debug.LogError("PlayerInputManager is missing from the scene.");
            return;
        }

        controls = PlayerInputManager.Instance.InputActions;

        if (player == null)
            player = FindFirstObjectByType<Player>();

        controls.Car.CarExit.performed += OnCarExitPressed;

        SwitchToCharacterControls();
    }

    private void OnDestroy()
    {
        if (controls != null)
            controls.Car.CarExit.performed -= OnCarExitPressed;
    }

    private void OnCarExitPressed(InputAction.CallbackContext context)
    {
        if (!isInCar)
            return;

        ExitCar();
    }

    public void SwitchToCharacterControls()
    {
        if (controls == null)
            return;

        isInCar = false;
        currentCar = null;

        controls.Car.Disable();
        controls.Player.Enable();

        if (player != null)
            player.SetControlsEnabledTo(true);

        SetPlayerCamera();
    }

    public void SwitchToCarControls(Car_Interact car)
    {
        if (controls == null || car == null)
            return;

        isInCar = true;
        currentCar = car;

        controls.Player.Disable();
        controls.Car.Enable();

        if (player != null)
            player.SetControlsEnabledTo(false);

        SetCarCamera();
    }

    private void ExitCar()
    {
        if (currentCar == null)
            return;

        currentCar.GetOutOfTheCar();

        SwitchToCharacterControls();
    }

    private void SetPlayerCamera()
    {
        if (carCamera != null)
        {
            carCamera.tag = "Untagged";
            carCamera.SetActive(false);
        }

        if (playerCamera != null)
        {
            playerCamera.SetActive(true);
            playerCamera.tag = "MainCamera";
        }
    }

    private void SetCarCamera()
    {
        if (playerCamera != null)
        {
            playerCamera.tag = "Untagged";
            playerCamera.SetActive(false);
        }

        if (carCamera != null)
        {
            carCamera.SetActive(true);
            carCamera.tag = "MainCamera";
        }
    }
}