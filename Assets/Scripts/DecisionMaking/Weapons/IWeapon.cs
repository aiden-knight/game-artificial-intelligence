using System.Buffers;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

public class WeaponImpl : MonoBehaviour
{
    [SerializeField]
    WeaponSO data;

    float timeLastFire = 0.0f;
    IMuzzle weaponMuzzle;
    int ammo;

    private void Start()
    {
        ammo = data.startingAmmoCount;
        weaponMuzzle = gameObject.GetComponentInChildren<IMuzzle>();
        Assert.IsNotNull(weaponMuzzle);
        weaponMuzzle.SetPrefab(data.bulletPrefab);
        weaponMuzzle.SetShotSpeed(data.shotSpeed);
    }

    public void PullTrigger()
    {
        if(ammo == 0) { return; }

        if (Time.time > timeLastFire + data.firerate)
        {
            weaponMuzzle.Fire();
            timeLastFire = Time.time;
            ammo -= 1;
        }
    }

    public WeaponType GetWeaponType()
    {
        return data.type;
    }

    public int GetAmmoCount()
    {
        return ammo;
    }

    public void AddAmmo(int count)
    {
        ammo += count;
    }
}

