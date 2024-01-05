using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pickup : MonoBehaviour
{
    public static Action PickUpCollected;

    [SerializeField]
    private BasePickupEffect effect;

    private void Start()
    {
        PickUpCollected += Destroy;
    }

    void Destroy()
    {
        PickUpCollected -= Destroy;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Players")) return;

        effect.ApplyEffect(collision.gameObject);

        PickUpCollected.Invoke();
    }
}
