using System;
using UnityEngine;

public class StoveCounter : BaseCounter, IInteractable, IHasProgress
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryinReceipeSO[] fryinReceipeSOArray;
    [SerializeField] private BurningReceipeSO[] burningReceipeSOArray;

    private State state;
    
    private float fryingTimer;
    private FryinReceipeSO fryinReceipeSO;
    
    private float burningTimer;
    private BurningReceipeSO burningReceipeSO;

    private void Start()
    {
        state = State.Idle;
    }

    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNotmalized = fryingTimer / fryinReceipeSO.fryingTimerMax
                    });

                    if (fryingTimer > fryinReceipeSO.fryingTimerMax)
                    {
                        // fried
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(fryinReceipeSO.output, this);
                        
                        state = State.Fried;
                        burningTimer = 0f;
                        burningReceipeSO = GetButningReceipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNotmalized = burningTimer / burningReceipeSO.burningTimerMax
                    });

                    if (burningTimer > burningReceipeSO.burningTimerMax)
                    {
                        // fried
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(burningReceipeSO.output, this);

                        state = State.Burned;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });


                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNotmalized = 0f
                        });
                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }

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
                    // player is carrying something that can be fried
                    interactorTransform.GetComponent<IKitchenObjectParent>().GetKitchenObject().SetKitchenObjectParent(this);

                    fryinReceipeSO = GetFryingReceipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    state = State.Frying;
                    fryingTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNotmalized = fryingTimer / fryinReceipeSO.fryingTimerMax
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

                        state = State.Idle;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNotmalized = 0f
                        });
                    }
                }
            }
            else
            {
                // player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(interactorTransform.GetComponent<IKitchenObjectParent>());

                state = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = state
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNotmalized = 0f
                });
            }
        }
    }

    private bool HasReceipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryinReceipeSO fryingReceipeSO = GetFryingReceipeSOWithInput(inputKitchenObjectSO);
        return fryingReceipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryinReceipeSO fryingReceipeSO = GetFryingReceipeSOWithInput(inputKitchenObjectSO);

        if (fryingReceipeSO != null)
        {
            return fryingReceipeSO.output;
        }
        else
        {
            return null;
        }
    }

    private FryinReceipeSO GetFryingReceipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryinReceipeSO fryingReceipeSO in fryinReceipeSOArray)
        {
            if (fryingReceipeSO.input == inputKitchenObjectSO)
            {
                return fryingReceipeSO;
            }
        }
        return null;
    }

    private BurningReceipeSO GetButningReceipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningReceipeSO burningReceipeSO in burningReceipeSOArray)
        {
            if (burningReceipeSO.input == inputKitchenObjectSO)
            {
                return burningReceipeSO;
            }
        }
        return null;
    }

    public bool IsFried()
    {
        return state == State.Fried;
    }

    public Transform GetTransform() => transform;
}
