using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlatesCounter : BaseCounter, IInteractable
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;

    private void Update()
    {
        spawnPlateTimer += Time.deltaTime;

        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0;

            if (platesSpawnedAmount < platesSpawnedAmountMax)
            {
                platesSpawnedAmount++;

                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void Interact(Transform interactorTransform)
    {
        if (!interactorTransform.GetComponent<IKitchenObjectParent>().HasKitchenObject())
        {
            // player is empty handed
            if (platesSpawnedAmount > 0)
            {
                // there is at least one plate here
                platesSpawnedAmount--;

                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, interactorTransform.GetComponent<IKitchenObjectParent>());

                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public Transform GetTransform() => transform;
}
