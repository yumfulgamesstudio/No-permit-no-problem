using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PlayerMovement : MonoBehaviour
{

    #region Class Variables
    [Header("Components")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera playerCamera;
    public float RotationMismatch { get; private set; } = 0f;
    public bool IsRotatingToTarget { get; private set; } = false;

    [Header("Base Movement")]
    public float walkAccelaration = 0.15f;
    public float walkSpeed = 3f;
    public float runAccelaration = 0.25f;
    public float runSpeed = 6f;
    public float sprintAccelaration = 0.5f;
    public float sprintSpeed = 9f;
    public float inAirAccelaration = 0.15f;
    public float drag = 0.1f;
    public float inAirDrag = 5f;
    public float gravity = 25f;
    public float terminalVelocity = 50f;
    public float jumpSpeed = 1.0f;
    public float movingTreshold = 0.01f;

    [Header("Animation")]
    public float playerModelRotationSpeed = 10f;
    public float rotateTotTargetTime = 0.25f;

    [Header("Camera Settings")]
    public float lookSenseH = 0.1f;
    public float lookSenseV = 0.1f;
    public float lookLimitV = 89f;

    [Header("Enviroment details")]
    [SerializeField] private LayerMask groundLayerMask;

    private PlayerLocomotionInput playerLocomotionInput;
    private PlayerState playerState;
    private Player player;

    private Vector2 cameraRotation = Vector2.zero;
    private Vector2 playerTargetRotation = Vector2.zero;

    private bool jumpedLastFrame = false;
    private bool isRotatingClockwise = false;
    private float rotatingToTargetTimer = 0f;
    private float verticalVelocity = 0f;
    private float antiBump;
    private float stepOffset;

    private PlayerMovementState lastMovementState = PlayerMovementState.Falling;
    #endregion

    #region Startup
    private void Awake()
    {
        playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        playerState = GetComponent<PlayerState>();
        player = GetComponent<Player>();

        antiBump = sprintSpeed;
        stepOffset = characterController.stepOffset;
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    #endregion

    #region Update Logic
    private void Update()
    {
        if ((!GameManager.Instance.IsGamePlaying() && !GameManager.Instance.IsCooking()) || GameManager.Instance.IsGamePaused())
        {
            return;
        }
        if (!player.controlsEnabled) return;

        UpdateMovementState();
        HandleVerticalMovement();
        HandleLateralMovement();
    }

    private void UpdateMovementState()
    {
        lastMovementState = playerState.CurrentPlayerMovementState;

        bool canRun = CanRun();
        bool isMovementInput = playerLocomotionInput.MovementInput != Vector2.zero;
        bool isMovingLaterally = IsMovingLaterally();
        bool isSprinting = playerLocomotionInput.SprintToggleOn && isMovingLaterally;
        bool isWalking = isMovingLaterally && (!canRun || playerLocomotionInput.WalkToggleOn);
        bool isGrounded = IsGrounded();

        PlayerMovementState lateralState = isWalking ? PlayerMovementState.Walking :
                                           isSprinting ? PlayerMovementState.Sprinting :
                                           isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;
        playerState.SetPlayerMovementState(lateralState);

        // control airborn states
        if ((!isGrounded || jumpedLastFrame) && characterController.velocity.y > 0f)
        {
            playerState.SetPlayerMovementState(PlayerMovementState.Jumping);
            jumpedLastFrame = false;
            characterController.stepOffset = 0f;
        }
        else if ((!isGrounded || jumpedLastFrame) && characterController.velocity.y <= 0f)
        {
            playerState.SetPlayerMovementState(PlayerMovementState.Falling);
            jumpedLastFrame = false;
            characterController.stepOffset = 0f;
        }
        else
        {
            characterController.stepOffset = stepOffset;
        }
    }

    private void HandleVerticalMovement()
    {
        bool isGrounded = playerState.InGroundedState();

        verticalVelocity -= gravity * Time.deltaTime;

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -antiBump;
        }

        if (playerLocomotionInput.JumpPressed && isGrounded)
        {
            verticalVelocity += Mathf.Sqrt(jumpSpeed * 3 * gravity);
            jumpedLastFrame = true;
        }

        if (playerState.IsStateGroundedState(lastMovementState) && !isGrounded)
        {
            verticalVelocity += antiBump;
        }

        if (Mathf.Abs(verticalVelocity) > Mathf.Abs(terminalVelocity))
        {
            verticalVelocity = -1f * Mathf.Abs(terminalVelocity);
        }
    }

    private void HandleLateralMovement()
    {
        // references for current state
        bool isSprinting = playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
        bool isGrounded = playerState.InGroundedState();
        bool isWalking = playerState.CurrentPlayerMovementState == PlayerMovementState.Walking;


        // state dependent acceleration and speed
        float lateralAccelaration = !isGrounded ? inAirAccelaration :
                                    isWalking ? walkAccelaration :
                                    isSprinting ? sprintAccelaration : runAccelaration;
        float clampedLateralMagnitude = !isGrounded ? sprintSpeed :
                                        isWalking ? walkSpeed :
                                        isSprinting ? sprintSpeed : runSpeed;

        Vector3 cameraForwardXZ = new Vector3(playerCamera.transform.forward.x, 0f, playerCamera.transform.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(playerCamera.transform.right.x, 0f, playerCamera.transform.right.z).normalized;
        Vector3 movementDirection = cameraForwardXZ * playerLocomotionInput.MovementInput.y + cameraRightXZ * playerLocomotionInput.MovementInput.x;

        Vector3 movementDelta = movementDirection * lateralAccelaration * Time.deltaTime;
        Vector3 newVelocity = characterController.velocity + movementDelta;

        // add drag
        float dragMagnitude = isGrounded ? drag : inAirDrag;
        Vector3 currentDrag = newVelocity.normalized * dragMagnitude * Time.deltaTime;
        newVelocity = (newVelocity.magnitude > dragMagnitude * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(new Vector3(newVelocity.x, 0f, newVelocity.z), clampedLateralMagnitude);
        newVelocity.y += verticalVelocity;
        newVelocity = !isGrounded ? HandleSteepWalls(newVelocity) : newVelocity;

        // move character
        characterController.Move(newVelocity * Time.deltaTime);
    }

    private Vector3 HandleSteepWalls(Vector3 velocity)
    {
        Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(characterController, groundLayerMask);
        float angle = Vector3.Angle(normal, Vector3.up);
        bool validAngle = angle <= characterController.slopeLimit;

        if (!validAngle && verticalVelocity < 0f)
            velocity = Vector3.ProjectOnPlane(velocity, normal);

        return velocity;
    }

    #endregion

    #region Late update Logic
    private void LateUpdate()
    {
        if (!player.controlsEnabled) return;
        UpdateCameraRotation();
    }

    private void UpdateCameraRotation()
    {
        if ((!GameManager.Instance.IsGamePlaying() && !GameManager.Instance.IsCooking()) || GameManager.Instance.IsGamePaused())
        {
            return;
        }

        cameraRotation.x += lookSenseH * playerLocomotionInput.LookInput.x;
        cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookSenseV * playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

        playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * playerLocomotionInput.LookInput.x;

        float rotationTolerance = 90f;
        bool isIdling = playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
        IsRotatingToTarget = rotatingToTargetTimer > 0;

        //rotate if we are not idling
        if (!isIdling)
        {
            RotatePlayerToTarget();
        }
        // if rotation mismatch not within tolerance, or rotate to target is active, rotate player model
        else if (Mathf.Abs(RotationMismatch) > rotationTolerance || IsRotatingToTarget)
        {

            UpdateIdleRotation(rotationTolerance);
        }

        playerCamera.transform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0f);

        // get angle between player and camera
        Vector3 camForwardProjectedXZ = new Vector3(playerCamera.transform.forward.x, 0f, playerCamera.transform.forward.z).normalized;
        Vector3 crossProduct = Vector3.Cross(transform.forward, camForwardProjectedXZ);
        float sign = Mathf.Sign(Vector3.Dot(crossProduct, transform.up));
        RotationMismatch = Vector3.Angle(transform.forward, camForwardProjectedXZ) * sign;
    }

    private void UpdateIdleRotation(float rotationTolerance)
    {
        // initiate new rotation direction if not already rotating
        if (Mathf.Abs(RotationMismatch) > rotationTolerance)
        {
            rotatingToTargetTimer = rotateTotTargetTime;
            isRotatingClockwise = RotationMismatch > rotationTolerance;
        }
        rotatingToTargetTimer -= Time.deltaTime;

        if (isRotatingClockwise && RotationMismatch > 0f || !isRotatingClockwise && RotationMismatch < 0f)
        {
            // rotate player
            RotatePlayerToTarget();
        }
    }

    private void RotatePlayerToTarget()
    {
        Quaternion targetRotationX = Quaternion.Euler(0f, playerTargetRotation.x, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotationX, playerModelRotationSpeed * Time.deltaTime);
    }
    #endregion

    #region State Check
    private bool IsMovingLaterally()
    {
        Vector3 lateralVelocity = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z);
        return lateralVelocity.magnitude > movingTreshold;
    }

    private bool IsGrounded()
    {
        bool grounded = playerState.InGroundedState() ? IsGroundedWhileGrounded() : IsGroundedWhileAirborne();

        return grounded;
    }


    private bool IsGroundedWhileGrounded()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - characterController.radius, transform.position.z);

        bool grounded = Physics.CheckSphere(spherePosition, characterController.radius, groundLayerMask, QueryTriggerInteraction.Ignore);

        return grounded;
    }

    private bool IsGroundedWhileAirborne()
    {
        Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(characterController, groundLayerMask);
        float angle = Vector3.Angle(normal, Vector3.up);
        bool validAngle = angle <= characterController.slopeLimit;

        return characterController.isGrounded && validAngle;
    }

    private bool CanRun()
    {
        // this means player is moving diagonally at 45 degrees or forward, if so, we can run
        return playerLocomotionInput.MovementInput.y >= Mathf.Abs(playerLocomotionInput.MovementInput.x);
    }
    #endregion

}
