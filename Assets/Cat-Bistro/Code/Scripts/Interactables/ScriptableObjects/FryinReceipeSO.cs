using UnityEngine;

[CreateAssetMenu()]
public class FryinReceipeSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float fryingTimerMax;
}
