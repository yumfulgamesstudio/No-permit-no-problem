using UnityEngine;

public class DeliveryCounter : BaseCounter, IInteractable
{
    public static DeliveryCounter Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public override void Interact(Transform interactorTransform)
    {
        if (interactorTransform.GetComponent<IKitchenObjectParent>().HasKitchenObject())
        {
            if (interactorTransform.GetComponent<IKitchenObjectParent>().GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                // only accepts plates
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);

                interactorTransform.GetComponent<IKitchenObjectParent>().GetKitchenObject().DestroySelf();
            }
        }
    }
    
    public Transform GetTransform() => transform;
}
