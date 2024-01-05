using UnityEngine;

[CreateAssetMenu(fileName = "PickupData", menuName = "ScriptableObjects/HealthPickup", order = 1)]

public class HealthPickUp : BasePickupEffect
{
    [SerializeField]
    int healthRestore = 1;

    public override void ApplyEffect(GameObject target)
    {
        target.GetComponent<Health>().ApplyHealing(healthRestore);
    }        
}
