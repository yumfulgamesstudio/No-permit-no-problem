using System;
using UnityEngine;

public class Task_CarToDeliver : MonoBehaviour
{

    public static event Action OnCarDelivery;

    public void InvokeOnCarDelivery() => OnCarDelivery?.Invoke();
}
