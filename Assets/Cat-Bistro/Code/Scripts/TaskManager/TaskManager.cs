using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    [Header("Tasks")]
    [SerializeField] private Task[] tasks;

    private int currentTaskIndex = 0;
    private Task currentTask;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCurrentTask();
    }

    private void Update()
    {
        if (currentTask == null)
        {
            return;
        }

        currentTask.UpdateTask();

        if (currentTask.TaskCompleted())
        {
            GoToNextTask();
        }
    }

    private void StartCurrentTask()
    {
        if (tasks == null || tasks.Length == 0)
        {
            Debug.LogWarning("No tasks assigned to TaskManager.");
            GameManager.Instance.SetGameOver();
            return;
        }

        if (currentTaskIndex >= tasks.Length)
        {
            GameManager.Instance.SetGameOver();
            return;
        }

        currentTask = tasks[currentTaskIndex];

        if (currentTask == null)
        {
            GoToNextTask();
            return;
        }

        currentTask.StartTask();
    }

    private void GoToNextTask()
    {
        currentTask.EndTask();

        currentTaskIndex++;
        StartCurrentTask();
    }

    public bool AllTasksCompleted()
    {
        return currentTaskIndex >= tasks.Length;
    }

    public Task GetCurrentTask()
    {
        return currentTask;
    }
}