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
        Vector2 dir = Maths.Normalise(m_PursuingEntity.transform.position - transform.position);
        m_DesiredVelocity = dir * m_Manager.m_Entity.m_MaxSpeed;

        return m_Weight * (m_DesiredVelocity - m_Manager.m_Entity.m_Velocity);
    }
}
