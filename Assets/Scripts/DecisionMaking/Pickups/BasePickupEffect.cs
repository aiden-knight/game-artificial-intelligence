using UnityEngine;

public abstract class BasePickupEffect : ScriptableObject
{
    public abstract void ApplyEffect(GameObject target);
}
