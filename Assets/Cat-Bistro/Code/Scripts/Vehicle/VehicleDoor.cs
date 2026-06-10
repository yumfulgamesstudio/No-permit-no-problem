using System.Collections;
using UnityEngine;

public class VehicleDoor : MonoBehaviour
{
    [Header("References")]
    public GameObject door;

    [Header("Settings")]
    [SerializeField] private float openingSpeed = 2f;
    [SerializeField] private bool isOpen = false;

    [Header("Custom Door Rotations")]
    [SerializeField] private Vector3 closedRotationEuler;
    [SerializeField] private Vector3 openedRotationEuler;

    private Quaternion closedRotation;
    private Quaternion openedRotation;
    private Coroutine currentCoroutine;

    private void Start()
    {
        if (door == null)
        {
            Debug.LogError("Door reference is missing on " + gameObject.name);
            return;
        }

        closedRotation = Quaternion.Euler(closedRotationEuler);
        openedRotation = Quaternion.Euler(openedRotationEuler);

        door.transform.localRotation = isOpen ? openedRotation : closedRotation;
    }

    public void RotateDoor()
    {
        if (door == null)
            return;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ToggleDoor());
    }

    public void OpenDoor()
    {
        if (door == null)
            return;

        if (isOpen)
            return;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(OpenDoorCoroutine());
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    private IEnumerator ToggleDoor()
    {
        Quaternion targetRotation = isOpen ? closedRotation : openedRotation;
        isOpen = !isOpen;

        while (Quaternion.Angle(door.transform.localRotation, targetRotation) > 0.01f)
        {
            door.transform.localRotation = Quaternion.Lerp(
                door.transform.localRotation,
                targetRotation,
                Time.deltaTime * openingSpeed
            );

            yield return null;
        }

        door.transform.localRotation = targetRotation;
        currentCoroutine = null;
    }

    private IEnumerator OpenDoorCoroutine()
    {
        Quaternion targetRotation = openedRotation;
        isOpen = true;

        while (Quaternion.Angle(door.transform.localRotation, targetRotation) > 0.01f)
        {
            door.transform.localRotation = Quaternion.Lerp(
                door.transform.localRotation,
                targetRotation,
                Time.deltaTime * openingSpeed
            );

            yield return null;
        }

        door.transform.localRotation = targetRotation;
        currentCoroutine = null;
    }
}