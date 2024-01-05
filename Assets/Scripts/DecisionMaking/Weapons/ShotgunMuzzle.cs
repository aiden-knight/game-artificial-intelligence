using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGunMuzzle : MonoBehaviour, IMuzzle
{
    GameObject bulletPrefab;

    float shotSpeed = 1.0f;

    public void Fire()
    {
        for (int i = 0; i < 4; i++)
        {
            Vector2 spread = transform.right * Random.Range(-0.5f, 0.5f);
            Vector2 offset = (Vector2)transform.up + spread;

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = offset.normalized * shotSpeed;
        }
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
