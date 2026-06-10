using UnityEngine;
using UnityEngine.UI;

public class ClearCounter : BaseCounter, IInteractable
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Transform interactorTransform)
    {
        if (!HasKitchenObject())
        {
            // there is no kitchen object on the counter
            if (interactorTransform.GetComponent<IKitchenObjectParent>().HasKitchenObject())
            {
                // player is carrying something
                interactorTransform.GetComponent<IKitchenObjectParent>().GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                // player is not carrying anything

            }
        }
        else
        {
            // there is a kitchen object on the counter
            if (interactorTransform.GetComponent<IKitchenObjectParent>().HasKitchenObject())
            {
                // player is carrying something
                if (interactorTransform.GetComponent<IKitchenObjectParent>().GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                }
                else
                {
                    // player is not carrying a plate
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        //counter is holding a plate
                        if (plateKitchenObject.TryAddIngredient(interactorTransform.GetComponent<IKitchenObjectParent>().GetKitchenObject().GetKitchenObjectSO()))
                        {
                            interactorTransform.GetComponent<IKitchenObjectParent>().GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else
            {
                // player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(interactorTransform.GetComponent<IKitchenObjectParent>());
            }
        }
    }

    public Transform GetTransform() => transform;
}
