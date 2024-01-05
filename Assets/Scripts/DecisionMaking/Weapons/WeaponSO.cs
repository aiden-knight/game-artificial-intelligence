using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum WeaponType
{
    Handgun = 0,
    Machinegun = 1,
    Shotgun = 2,
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponDataObject", order = 1)]
public class WeaponSO : ScriptableObject
{
    [SerializeField]
    public GameObject bulletPrefab;

    [SerializeField]
    public float firerate;

    [SerializeField]
    public WeaponType type;

    [SerializeField]
    public int startingAmmoCount;

    [SerializeField]
    public int shotSpeed;
}