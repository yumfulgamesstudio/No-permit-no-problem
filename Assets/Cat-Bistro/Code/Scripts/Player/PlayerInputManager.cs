using UnityEngine;

[DefaultExecutionOrder(-3)]
public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance;

    public InputSystem_Actions InputActions { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        InputActions?.Enable();
    }

    private void OnDisable()
    {
        InputActions?.Disable();
    }
}