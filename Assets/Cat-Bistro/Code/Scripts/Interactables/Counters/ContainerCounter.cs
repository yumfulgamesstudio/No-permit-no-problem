using System;
using UnityEngine;

public class ContainerCounter : BaseCounter, IInteractable
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public event EventHandler OnPlayerGrabbedObject;

    public override void Interact(Transform interactorTransform)
    {
        if (!interactorTransform.GetComponent<IKitchenObjectParent>().HasKitchenObject())
        {
            // player is not carrying anything
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, interactorTransform.GetComponent<IKitchenObjectParent>());

            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }

    public Transform GetTransform() => transform;
}
