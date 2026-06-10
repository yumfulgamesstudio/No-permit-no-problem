using System;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerInteract : MonoBehaviour, IKitchenObjectParent
{
    [Header("Raycast (look) interact")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private LayerMask interactLayerMask = ~0; // everything by default

    //[SerializeField] private GameObject interactionUIElement;
    //[SerializeField] private GameObject cuttingUIElement;
    //[SerializeField] public CuttingCounter cuttingCounter;
     




    private IInteractable currentInteractable;
    private IHighlightable currentHighlightable;

    private KitchenObject kitchenObject;
    private PlayerLocomotionInput locomotionInput;

    public static PlayerInteract Instance { get; private set; }

    public event EventHandler OnPickedSomething;

    private void Awake()
    {
        Instance = this;

        if (playerCamera == null) playerCamera = Camera.main;
        locomotionInput = GetComponent<PlayerLocomotionInput>(); 

        
    }

    private void Update()
    {
        UpdateLookTarget();

        if (locomotionInput != null && locomotionInput.InteractPressed && currentInteractable != null)
        {
            if ((!GameManager.Instance.IsGamePlaying() && !GameManager.Instance.IsCooking()) || GameManager.Instance.IsGamePaused())
            {
                return;
            }

            currentInteractable.Interact(transform);
        }

        if (locomotionInput != null && locomotionInput.InteractAlternatePressed && currentInteractable != null)
        {
            if ((!GameManager.Instance.IsGamePlaying() && !GameManager.Instance.IsCooking()) || GameManager.Instance.IsGamePaused())
            {
                return;
            }
            currentInteractable.InteractAlternate(transform);
        }

        
    }

    private void UpdateLookTarget()
    {
        IInteractable newInteractable = null;
        IHighlightable newHighlightable = null;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayerMask))
        {

            newInteractable = hit.collider.GetComponentInParent<IInteractable>();

            if (newInteractable != null)
            {
                newHighlightable = (newInteractable as IHighlightable)
                                  ?? hit.collider.GetComponentInParent<IHighlightable>();
                
            }
            

        }

       // UpdateInteractionText();

        // If target changed, update highlight
        if (!ReferenceEquals(newInteractable, currentInteractable))
        {
            // turn off old
            if (currentHighlightable != null)
                currentHighlightable.SetHighlighted(false);
            

            currentInteractable = newInteractable;
            currentHighlightable = newHighlightable;

            // turn on new
            if (currentHighlightable != null)
                currentHighlightable.SetHighlighted(true);
            
        }
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
            //animation
        }

    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }


    //public void UpdateInteractionText()
    //{
    //    if (currentInteractable != null)
    //    {
    //        if (cuttingCounter.HasKitchenObject() != false && cuttingCounter.cuttingProgress >= 0)
    //        {
    //            cuttingUIElement.SetActive(true);
    //            interactionUIElement.SetActive(false);

    //        }
    //        else if (cuttingCounter.HasKitchenObject() == false && cuttingCounter.cuttingProgress >= 2)
    //        {
                
    //            cuttingUIElement.SetActive(false);
    //        }
    //        else
    //        {
    //            interactionUIElement.SetActive(true);
    //        }
            
    //    }

    //    else
    //    {
    //        interactionUIElement.SetActive (false);
    //        cuttingUIElement.SetActive(false);
    //    }

    //}

}
