using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerState playerState;

    private float footstepTimer;

    private float walkStepTime = 0.6f;
    private float runStepTime = 0.5f;
    private float sprintingStepTime = 0.3f;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerState = GetComponent<PlayerState>();
    }

    private void Update()
    {
        if (!playerState.InGroundedState()) return;

        PlayerMovementState state = playerState.CurrentPlayerMovementState;

        float stepTime;

        if (state == PlayerMovementState.Walking)
        {
            stepTime = walkStepTime;
        }
        else if (state == PlayerMovementState.Running)
        {
            stepTime = runStepTime;
        }
        else if (state == PlayerMovementState.Sprinting)
        {
            stepTime = sprintingStepTime;
        }
        else
        {
            return;
        }

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            footstepTimer = stepTime;

            float volume = state == PlayerMovementState.Sprinting ? 1.2f : 1f;

            SoundManager.Instance.PlayFootstepSound(transform.position, volume);
        }
    }
}