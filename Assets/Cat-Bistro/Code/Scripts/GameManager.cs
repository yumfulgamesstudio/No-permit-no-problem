using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private PlayerLocomotionInput locomotionInput;
    public Player player;

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;

    public event EventHandler OnCookingStarted;
    public event EventHandler OnCookingEnded;

    private enum State
    {
        WaitingToStart,
        Playing,
        Cooking,
        GameOver
    }

    private State state;

    private float waitingToStartTimer = 1f;

    [Header("Cooking")]
    [SerializeField] private float cookingTimerMax = 60f;
    private float cookingTimer;

    private bool isGamePaused = false;

    private void Awake()
    {
        Instance = this;

        state = State.WaitingToStart;

        locomotionInput = FindFirstObjectByType<PlayerLocomotionInput>();
        player = FindFirstObjectByType<Player>();
    }

    private void Update()
    {
        if (locomotionInput != null && locomotionInput.PausePressed)
        {
            OnPausePressed(this, EventArgs.Empty);
        }

        switch (state)
        {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;

                if (waitingToStartTimer < 0f)
                {
                    state = State.Playing;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

                break;

            case State.Playing:
                break;

            case State.Cooking:
                cookingTimer -= Time.deltaTime;

                if (cookingTimer <= 0f)
                {
                    StopCooking();
                }

                break;

            case State.GameOver:
                break;
        }

        Debug.Log(state);
    }

    private void OnPausePressed(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    public void StartCooking()
    {
        if (state == State.GameOver)
        {
            return;
        }

        cookingTimer = cookingTimerMax;
        state = State.Cooking;

        OnStateChanged?.Invoke(this, EventArgs.Empty);
        OnCookingStarted?.Invoke(this, EventArgs.Empty);

        Debug.Log("Cooking started.");
    }

    public void StopCooking()
    {
        if (state != State.Cooking)
        {
            return;
        }

        state = State.Playing;

        OnStateChanged?.Invoke(this, EventArgs.Empty);
        OnCookingEnded?.Invoke(this, EventArgs.Empty);

        Debug.Log("Cooking ended. Back to playing.");
    }

    public void SetGameOver()
    {
        state = State.GameOver;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void TogglePauseGame()
    {
        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsGamePlaying()
    {
        return state == State.Playing;
    }

    public bool IsCooking()
    {
        return state == State.Cooking;
    }

    public bool IsGameOver()
    {
        return state == State.GameOver;
    }

    public bool IsGamePaused()
    {
        return isGamePaused;
    }

    public float GetCookingTimerNormalized()
    {
        return 1 - (cookingTimer / cookingTimerMax);
    }

    public float GetCookingTimer()
    {
        return cookingTimer;
    }
}