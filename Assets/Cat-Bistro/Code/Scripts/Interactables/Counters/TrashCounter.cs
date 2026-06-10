using UnityEngine;
using System;

public class TrashCounter : BaseCounter, IInteractable
{
    public static event EventHandler OnAnyObjectTrashedHere;

    new public static void ResetStaticData()
    {
        OnAnyObjectTrashedHere = null;
    }

    public override void Interact(Transform interactorTransform)
    {
        if (interactorTransform.GetComponent<IKitchenObjectParent>().HasKitchenObject())
        {
            interactorTransform.GetComponent<IKitchenObjectParent>().GetKitchenObject().DestroySelf();

            OnAnyObjectTrashedHere?.Invoke(this, EventArgs.Empty);
        }
    }

    public Transform GetTransform() => transform;
}
