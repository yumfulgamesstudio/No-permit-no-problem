using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform playerBody;

    public InputSystem_Actions controls { get; private set; }
    public PlayerMovement movement { get; private set; }
    public PlayerInteract interaction { get; private set; }
    public Animator anim { get; private set; }

    public bool controlsEnabled { get; private set; }

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        movement = GetComponent<PlayerMovement>();
        interaction = GetComponent<PlayerInteract>();
    }

    private void Start()
    {
        controls = ControlsManager.instance.controls;
    }

    public void SetControlsEnabledTo(bool enabled)
    {
        controlsEnabled = enabled;
    }
}
