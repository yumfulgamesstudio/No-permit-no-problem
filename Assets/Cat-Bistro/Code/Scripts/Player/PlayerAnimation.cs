using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float locomotionBlendSpeed = 4f;

    private Player player;
    private PlayerLocomotionInput playerLocomotionInput;
    private PlayerState playerState;
    private PlayerMovement playerMovement;

    private static int inputXHash = Animator.StringToHash("inputX");
    private static int inputYHash = Animator.StringToHash("inputY");
    private static int inputMagnitudeHash = Animator.StringToHash("inputMagnitude");
    private static int isIdlingHash = Animator.StringToHash("isIdling");
    private static int isGroundedHash = Animator.StringToHash("isGrounded");
    private static int isFallingHash = Animator.StringToHash("isFalling");
    private static int isJumpingHash = Animator.StringToHash("isJumping");
    private static int isRotatingToTargetHash = Animator.StringToHash("isRotatingToTarget");
    private static int rotationMismatchHash = Animator.StringToHash("rotationMismatch");

    private Vector3 currentBlendInput = Vector3.zero;

    private float sprintMaxBlendValue = 1.5f;
    private float runMaxBlendValue = 1f;
    private float walkMaxBlendValue = 0.5f;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        playerState = GetComponent<PlayerState>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (!player.controlsEnabled)
        {
            ResetToIdleAnimation();
            return;
        }

        UpdateAnimationState();
    }

    private void ResetToIdleAnimation()
    {
        currentBlendInput = Vector3.Lerp(
            currentBlendInput,
            Vector3.zero,
            locomotionBlendSpeed * Time.deltaTime
        );

        playerAnimator.SetBool(isIdlingHash, true);
        playerAnimator.SetBool(isFallingHash, false);
        playerAnimator.SetBool(isJumpingHash, false);
        playerAnimator.SetBool(isRotatingToTargetHash, false);

        playerAnimator.SetFloat(inputXHash, currentBlendInput.x);
        playerAnimator.SetFloat(inputYHash, currentBlendInput.y);
        playerAnimator.SetFloat(inputMagnitudeHash, currentBlendInput.magnitude);
        playerAnimator.SetFloat(rotationMismatchHash, 0f);
    }

    private void UpdateAnimationState()
    {
        bool isIdling = playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
        bool isRunning = playerState.CurrentPlayerMovementState == PlayerMovementState.Running;
        bool isSprinting = playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
        bool isJumping = playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping;
        bool isFalling = playerState.CurrentPlayerMovementState == PlayerMovementState.Falling;
        bool isGrounded = playerState.InGroundedState();

        Vector2 inputTarget = isSprinting ? playerLocomotionInput.MovementInput * sprintMaxBlendValue :
                              isRunning ? playerLocomotionInput.MovementInput * runMaxBlendValue :
                                          playerLocomotionInput.MovementInput * walkMaxBlendValue;

        currentBlendInput = Vector3.Lerp(
            currentBlendInput,
            inputTarget,
            locomotionBlendSpeed * Time.deltaTime
        );

        playerAnimator.SetBool(isGroundedHash, isGrounded);
        playerAnimator.SetBool(isIdlingHash, isIdling);
        playerAnimator.SetBool(isFallingHash, isFalling);
        playerAnimator.SetBool(isJumpingHash, isJumping);
        playerAnimator.SetBool(isRotatingToTargetHash, playerMovement.IsRotatingToTarget);

        playerAnimator.SetFloat(inputXHash, currentBlendInput.x);
        playerAnimator.SetFloat(inputYHash, currentBlendInput.y);
        playerAnimator.SetFloat(inputMagnitudeHash, currentBlendInput.magnitude);
        playerAnimator.SetFloat(rotationMismatchHash, playerMovement.RotationMismatch);
    }
}