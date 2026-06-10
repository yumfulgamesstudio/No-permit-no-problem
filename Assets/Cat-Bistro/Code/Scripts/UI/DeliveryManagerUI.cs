using System;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject deliveryVisual;
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    private void Awake()
    {
        recipeTemplate.gameObject.SetActive(false);
        deliveryVisual.SetActive(false);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCookingStarted += GameManager_OnCookingStarted;
            GameManager.Instance.OnCookingEnded += GameManager_OnCookingEnded;
        }

        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeSpawned(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeCompleted(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void GameManager_OnCookingStarted(object sender, EventArgs e)
    {
        deliveryVisual.SetActive(true);
        UpdateVisual();
    }

    private void GameManager_OnCookingEnded(object sender, EventArgs e)
    {
        deliveryVisual.SetActive(false);
    }

    private void UpdateVisual()
    {
        foreach (Transform child in container)
        {
            if (child == recipeTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList())
        {
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            recipeTransform.gameObject.SetActive(true);
            recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipeSO);
        }
    }

    private void OnDestroy()
    {
        if (DeliveryManager.Instance != null)
        {
            DeliveryManager.Instance.OnRecipeSpawned -= DeliveryManager_OnRecipeSpawned;
            DeliveryManager.Instance.OnRecipeCompleted -= DeliveryManager_OnRecipeCompleted;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnCookingStarted -= GameManager_OnCookingStarted;
            GameManager.Instance.OnCookingEnded -= GameManager_OnCookingEnded;
        }
    }
}