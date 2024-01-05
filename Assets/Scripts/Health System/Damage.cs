using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ApplyDamage : MonoBehaviour
{
    public Action OnDamageDealt;

    [SerializeField]
    int _damage;

    [SerializeField]
    bool destroyOnApply = false;

    [SerializeField]
    LayerMask canDamage = 0;

#if DEBUG
    private void Start()
    {
        Assert.AreNotEqual(canDamage, 0);
    }
#endif

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((canDamage.value & 1 << collision.gameObject.layer) != 0)
        {
            Health mv = collision.gameObject.GetComponent<Health>();
            if (mv != null)
            {
                mv.ApplyDamage(_damage);
                OnDamageDealt?.Invoke();
            }
        }
        if (destroyOnApply)
        {
            Destroy(gameObject);
        }
    }
}
