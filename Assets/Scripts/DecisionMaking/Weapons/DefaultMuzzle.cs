using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMuzzle : MonoBehaviour, IMuzzle
{
    GameObject bulletPrefab;

    float shotSpeed = 1.0f;

    public void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = transform.up * shotSpeed;
    }

    public void SetPrefab(GameObject prefab)
    {
        bulletPrefab = prefab;
    }

    public void SetShotSpeed(float speed)
    {
        shotSpeed = speed;
    }
}