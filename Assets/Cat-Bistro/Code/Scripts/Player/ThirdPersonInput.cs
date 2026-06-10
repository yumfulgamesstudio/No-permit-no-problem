using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)]
public class ThirdPersonInput : MonoBehaviour, InputSystem_Actions.IThirsPersonMapActions
{
    #region Class Variables

    public Vector2 ScrollInput { get; private set; }

    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private float cameraZoomSpeed = 0.1f;
    [SerializeField] private float cameraMinZoom = 1f;
    [SerializeField] private float cameraMaxZoom = 5f;

    private CinemachineThirdPersonFollow thirdPersonFollow;

    #endregion

    #region Startup
    private void Awake()
    {
        thirdPersonFollow = virtualCamera.GetComponent<CinemachineThirdPersonFollow>();
    }

    private void OnEnable()
    {
        if (PlayerInputManager.Instance?.InputActions == null)
        {
            Debug.LogError("Player controls are not initialized - cannot enable");
            return;
        }

        PlayerInputManager.Instance.InputActions.ThirsPersonMap.Enable();
        PlayerInputManager.Instance.InputActions.ThirsPersonMap.SetCallbacks(this);
    }

    private void OnDisable()
    {
        if (PlayerInputManager.Instance?.InputActions == null)
        {
            Debug.LogError("Player controls are not initialized - cannot disable");
            return;
        }

        PlayerInputManager.Instance.InputActions.Disable();
        PlayerInputManager.Instance.InputActions.ThirsPersonMap.RemoveCallbacks(this);
    }
    #endregion

    #region Update Logic
    private void Update()
    {
        thirdPersonFollow.CameraDistance = Mathf.Clamp(thirdPersonFollow.CameraDistance + ScrollInput.y, cameraMinZoom, cameraMaxZoom);
    }

    private void LateUpdate()
    {
        ScrollInput = Vector2.zero;
    }
    #endregion

    #region Input Callbacks

    public void OnScrollCamera(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        Vector2 scrollInput = context.ReadValue<Vector2>();
        ScrollInput = -1f * scrollInput.normalized * cameraZoomSpeed;
    }
    #endregion
}
