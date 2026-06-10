using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact(Transform interactorTransform);

    void InteractAlternate(Transform interactorTransform);

    Transform GetTransform();
}

public interface IHighlightable
{
    void SetHighlighted(bool highlighted);
}

public interface IKitchenObjectParent
{
    public Transform GetKitchenObjectFollowTransform();

    public void SetKitchenObject(KitchenObject kitchenObject);

    public KitchenObject GetKitchenObject();

    public void ClearKitchenObject();

    public bool HasKitchenObject();
}