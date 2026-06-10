using UnityEngine;
using UnityEngine.Events;

public class VehicleDoorsInteract : MonoBehaviour, IInteractable, IHighlightable
{
    [Header("References")]
    private Vehicle vehicle;
    [SerializeField] private VehicleDoor vehicleDoor;

    private Outline outline;

    private void Awake()
    {
        if (vehicle == null)
            vehicle = GetComponentInParent<Vehicle>();

        outline = GetComponent<Outline>();

        SetHighlighted(false);
    }

    public void Interact(Transform interactorTransform)
    {
        Debug.Log("Door interaction called");

        if (vehicleDoor == null)
        {
            Debug.LogError("VehicleDoor reference is missing on " + gameObject.name, this);
            return;
        }

        vehicleDoor.RotateDoor();
    }

    public Transform GetTransform() => transform;

    public void SetHighlighted(bool highlighted)
    {
        if (outline != null)
            outline.enabled = highlighted;
    }

    public void InteractAlternate(Transform interactorTransform)
    {
        // Leave empty if unused
    }
}
