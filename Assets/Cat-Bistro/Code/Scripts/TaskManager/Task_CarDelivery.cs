using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Key Task", menuName = "Missions/Car delivery - Mission")]
public class Task_CarDelivery : Task
{
    private bool carWasDelivered;

    public override void StartTask()
    {
        carWasDelivered = false;

        string missionText = "Drive to the park and park in the marked spot.";
        string missionDetails = "";

        if (InGameUI.Instance != null)
        {
            InGameUI.Instance.UpdateTaskInfo(missionText, missionDetails);
        }

        Task_CarToDeliver.OnCarDelivery += CarDeliveryCompleted;

        Car car = FindAnyObjectByType<Car>();

        if (car != null && car.GetComponent<Task_CarToDeliver>() == null)
        {
            car.AddComponent<Task_CarToDeliver>();
        }
    }

    public override bool TaskCompleted()
    {
        return carWasDelivered;
    }

    private void CarDeliveryCompleted()
    {
        carWasDelivered = true;

        Task_CarToDeliver.OnCarDelivery -= CarDeliveryCompleted;

        if (InGameUI.Instance != null)
        {
            InGameUI.Instance.UpdateTaskInfo("nice", "");
        }
    }
}