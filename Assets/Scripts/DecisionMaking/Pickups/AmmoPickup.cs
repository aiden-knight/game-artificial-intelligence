using UnityEngine;

[CreateAssetMenu(fileName = "PickupData", menuName = "ScriptableObjects/AmmoPickup", order = 1)]
public class AmmoPickUp : BasePickupEffect
{
    [SerializeField]
    int ammoToAdd = 10;

    public override void ApplyEffect(GameObject target)
    {
        WeaponImpl[] weapons = target.GetComponentsInChildren<WeaponImpl>(true);

        foreach (WeaponImpl weapon in weapons)
        {
            weapon.AddAmmo(ammoToAdd);
        }
    }
}
