using UnityEngine;
using UnityEngine.Events;

public class VehicleInteract : MonoBehaviour, IInteractable, IHighlightable

{
    
    [SerializeField] private Vehicle vehicle;
    [SerializeField] private TagHandle playerTag;
    

    public UnityEvent onInteraction;

    private Outline outline;
    //private Animator animator;
    

    private void Awake()
    {
        //animator = GetComponent<Animator>();
        //vehicle = GetComponent<Vehicle>();
        outline = GetComponent<Outline>();

        SetHighlighted(false);
    }

    public void Interact(Transform interactorTransform)
    {
        Debug.Log("VehicleInteract.Interact called");

        if (vehicle == null)
        {
            Debug.LogError("Vehicle reference is missing on VehicleInteract!");
            return;
        }

        vehicle.enabled = true;
        
        onInteraction.Invoke();
        vehicle.StartVehicle();
        
    }

    public Transform GetTransform() => transform;

    public void SetHighlighted(bool highlighted)
    {
        if (outline != null) outline.enabled = highlighted;
    }

    public void InteractAlternate(Transform interactorTransform)
    {
        throw new System.NotImplementedException();
    }
}
