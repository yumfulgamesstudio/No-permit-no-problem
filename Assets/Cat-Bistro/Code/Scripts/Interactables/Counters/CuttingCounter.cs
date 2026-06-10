using System;
using UnityEngine;

public class CuttingCounter : BaseCounter, IInteractable, IHasProgress
{
    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;
    
    [SerializeField] private CuttingReceipeSO[] cuttingReceipeSOArray;

    public int cuttingProgress;

    public override void Interact(Transform interactorTransform)
    {
        if (!HasKitchenObject())
        {
            // there is no kitchen object on the counter
            if (interactorTransform.GetComponent<IKitchenObjectParent>().HasKitchenObject())
            {
                // player is carrying something
                if (HasReceipeWithInput(interactorTransform.GetComponent<IKitchenObjectParent>().GetKitchenObject().GetKitchenObjectSO()))
                {
                    // player is carrying something that can be cut
                    interactorTransform.GetComponent<IKitchenObjectParent>().GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;

                    CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNotmalized = (float)cuttingProgress / cuttingReceipeSO.cuttingProgressMax
                    });
                }
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
            }
            else
            {
                // player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(interactorTransform.GetComponent<IKitchenObjectParent>());
            }
        }
    }

    public override void InteractAlternate(Transform interactorTransform)
    {
        if (HasKitchenObject() && HasReceipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            // there is a kitchenObject AND it can be cut
            cuttingProgress++;

            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);

            CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNotmalized = (float)cuttingProgress / cuttingReceipeSO.cuttingProgressMax
            });

            if (cuttingProgress >= cuttingReceipeSO.cuttingProgressMax)
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }
        }
    }

    private bool HasReceipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOWithInput(inputKitchenObjectSO);
        return cuttingReceipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOWithInput(inputKitchenObjectSO);
        
        if (cuttingReceipeSO != null)
        {
            return cuttingReceipeSO.output;
        }
        else
        {
            return null;
        }
    }

    private CuttingReceipeSO GetCuttingReceipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingReceipeSO cuttingReceipeSO in cuttingReceipeSOArray)
        {
            if (cuttingReceipeSO.input == inputKitchenObjectSO)
            {
                return cuttingReceipeSO;
            }
        }
        return null;
    }

    public Transform GetTransform() => transform;
}
