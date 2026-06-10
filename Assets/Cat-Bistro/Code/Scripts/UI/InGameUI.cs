using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    public static InGameUI Instance;

    [Header("Tasks")]
    [SerializeField] private GameObject taskUI;
    [SerializeField] private TextMeshProUGUI taskText;
    [SerializeField] private TextMeshProUGUI taskDetails;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateTaskInfo(string taskText, string taskDetails = null)
    {
        this.taskText.text = taskText;
        this.taskDetails.text = taskDetails ?? "";
    }

    public void ShowTaskUI()
    {
        if (taskUI != null)
        {
            taskUI.SetActive(true);
        }
    }

    public void HideTaskUI()
    {
        if (taskUI != null)
        {
            taskUI.SetActive(false);
        }
    }
}