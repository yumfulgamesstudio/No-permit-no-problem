using System;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private GameObject clockVisual;
    [SerializeField] private Image timerImage;

    private void Start()
    {
        Hide();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCookingStarted += GameManager_OnCookingStarted;
            GameManager.Instance.OnCookingEnded += GameManager_OnCookingEnded;
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsCooking())
        {
            return;
        }

        timerImage.fillAmount = GameManager.Instance.GetCookingTimerNormalized();
    }

    private void GameManager_OnCookingStarted(object sender, EventArgs e)
    {
        Show();
    }

    private void GameManager_OnCookingEnded(object sender, EventArgs e)
    {
        Hide();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCookingStarted -= GameManager_OnCookingStarted;
            GameManager.Instance.OnCookingEnded -= GameManager_OnCookingEnded;
        }
    }

    public void Show()
    {
        clockVisual.SetActive(true);
    }

    public void Hide()
    {
        clockVisual.SetActive(false);
    }
}