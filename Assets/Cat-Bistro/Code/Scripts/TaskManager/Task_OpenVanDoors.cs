using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Open Van Doors Task", menuName = "Missions/Open Van Doors")]
public class Task_OpenVanDoors : Task
{
    private CarDoorsTaskTarget vanDoorsTaskTarget;

    private bool cookingStarted;
    private bool cookingFinished;

    public override void StartTask()
    {
        cookingStarted = false;
        cookingFinished = false;

        if (InGameUI.Instance != null)
        {
            InGameUI.Instance.ShowTaskUI();

            InGameUI.Instance.UpdateTaskInfo(
                "Open the van doors and start your shift",
                ""
            );
        }

        vanDoorsTaskTarget = FindFirstObjectByType<CarDoorsTaskTarget>();

        if (vanDoorsTaskTarget == null)
        {
            Debug.LogError("No CarDoorsTaskTarget found in scene.");
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCookingEnded -= CookingEnded;
            GameManager.Instance.OnCookingEnded += CookingEnded;
        }
    }

    public override void UpdateTask()
    {
        if (vanDoorsTaskTarget == null)
        {
            return;
        }

        if (!cookingStarted && vanDoorsTaskTarget.AreBothDoorsOpen())
        {
            cookingStarted = true;

            if (InGameUI.Instance != null)
            {
                InGameUI.Instance.HideTaskUI();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartCooking();
            }
        }
    }

    public override bool TaskCompleted()
    {
        return cookingFinished;
    }

    public override void EndTask()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCookingEnded -= CookingEnded;
        }
    }

    private void CookingEnded(object sender, EventArgs e)
    {
        cookingFinished = true;
    }
}