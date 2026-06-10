using UnityEngine;

public class CarDoorsTaskTarget : MonoBehaviour
{
    [SerializeField] private VehicleDoor windowDoorUp;
    [SerializeField] private VehicleDoor windowDoorDown;

    public void OpenBothDoors()
    {
        if (windowDoorUp != null)
        {
            windowDoorUp.OpenDoor();
        }

        if (windowDoorDown != null)
        {
            windowDoorDown.OpenDoor();
        }
    }

    public bool AreBothDoorsOpen()
    {
        if (windowDoorUp == null || windowDoorDown == null)
        {
            return false;
        }

        return windowDoorUp.IsOpen() && windowDoorDown.IsOpen();
    }
}
