using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Pursuit : SteeringBehaviour
{
    [Header("Pursuit Properties")]
    [Header("Settings")]
    public MovingEntity m_PursuingEntity;

    public override Vector2 CalculateForce()
    {
        //delete me
        return Vector2.zero;
    }
}
