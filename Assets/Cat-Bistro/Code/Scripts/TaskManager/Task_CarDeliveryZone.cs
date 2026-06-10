using UnityEngine;

public class Task_CarDeliveryZone : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Car car = other.GetComponentInParent<Car>();

        if (car != null)
            car.GetComponent<Task_CarToDeliver>().InvokeOnCarDelivery();
    }

}
