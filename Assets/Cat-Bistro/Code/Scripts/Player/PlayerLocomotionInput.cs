using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)]
public class PlayerLocomotionInput : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    #region Class Variables
    [SerializeField] private bool holdToSprint = true;

    public Vector2 MovementInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool SprintToggleOn { get; private set; }
    public bool WalkToggleOn { get; private set; }
    public bool InteractPressed { get; private set; }
    public bool InteractAlternatePressed { get; private set; }
    public bool PausePressed { get; private set; }
    #endregion

    #region Startup
    private void OnEnable()
    {
        if (PlayerInputManager.Instance?.InputActions == null)
        {
            Debug.LogError("Player controls are not initialized - cannot enable");
            return;
        }

        PlayerInputManager.Instance.InputActions.Player.SetCallbacks(this);
        PlayerInputManager.Instance.InputActions.Player.Enable();
    }

    private void OnDisable()
    {
        if (PlayerInputManager.Instance?.InputActions == null)
            return;

        PlayerInputManager.Instance.InputActions.Player.RemoveCallbacks(this);
        PlayerInputManager.Instance.InputActions.Player.Disable();
    }
    #endregion

    #region Late Update Logic
    private void LateUpdate()
    {
        JumpPressed = false;
        InteractPressed = false;
        InteractAlternatePressed = false;
        PausePressed = false;
    }
    #endregion

    #region Input Callbacks
    public void OnMove(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SprintToggleOn = holdToSprint || !SprintToggleOn;
        }
        else if (context.canceled)
        {
            SprintToggleOn = !holdToSprint && SprintToggleOn;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        JumpPressed = true;
    }

    public void OnToggleWalk(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        WalkToggleOn = !WalkToggleOn;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        InteractPressed = true;
    }

    public void OnInteractAlternate(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        InteractAlternatePressed = true;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        PausePressed = true;
    }
    #endregion
}
