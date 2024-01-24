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
        if(m_PursuingEntity == null) return Vector2.zero;

        Vector2 toEntity = m_PursuingEntity.transform.position - transform.position;
        float opponentSpeed = Maths.Magnitude(m_PursuingEntity.m_Velocity);
        Vector2 chasePos;

        if (opponentSpeed > 0)
        {
            float predictionTime = Maths.Magnitude(toEntity) / (m_Manager.m_Entity.m_MaxSpeed + opponentSpeed);
            chasePos = (Vector2)m_PursuingEntity.transform.position + m_PursuingEntity.m_Velocity * predictionTime;
        }
        else
        {
            chasePos = m_PursuingEntity.transform.position;
        }

        Vector2 dir = Maths.Normalise(chasePos - (Vector2)transform.position);
        m_DesiredVelocity = dir * m_Manager.m_Entity.m_MaxSpeed;

        return m_Weight * (m_DesiredVelocity - m_Manager.m_Entity.m_Velocity);
    }
}
