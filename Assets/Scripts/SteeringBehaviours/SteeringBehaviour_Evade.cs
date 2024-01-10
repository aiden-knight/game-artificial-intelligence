using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Evade : SteeringBehaviour
{
    [Header("Evade Properties")]
    [Header("Settings")]
    public MovingEntity m_EvadingEntity;
    public float m_EvadeRadius;

    [Space(10)]

    [Header("Debugs")]
    [SerializeField]
    protected Color m_Debug_RadiusColour = Color.yellow;

    public override Vector2 CalculateForce()
    {
        if (m_EvadingEntity == null) return Vector2.zero;

        Vector2 betweenVector = m_EvadingEntity.transform.position - m_Manager.m_Entity.transform.position;
        if (Maths.Magnitude(betweenVector) > m_EvadeRadius) return Vector2.zero;

        float opponentSpeed = Maths.Magnitude(m_EvadingEntity.m_Velocity);
        Vector2 fleePos;

        if(opponentSpeed > 0)
        {
            float predictionTime = Maths.Magnitude(betweenVector) / (m_Manager.m_Entity.m_MaxSpeed + opponentSpeed);
            fleePos = (Vector2)m_EvadingEntity.transform.position + m_EvadingEntity.m_Velocity * predictionTime;
        }
        else
        {
            fleePos = m_EvadingEntity.transform.position;
        }

        Vector2 negativeDir = (Vector2)m_Manager.m_Entity.transform.position - fleePos;
        m_DesiredVelocity = negativeDir * m_Manager.m_Entity.m_MaxSpeed;

        return m_Weight * (m_DesiredVelocity - m_Manager.m_Entity.m_Velocity);
    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                Gizmos.color = m_Debug_RadiusColour;
                Gizmos.DrawWireSphere(transform.position, m_EvadeRadius);

                base.OnDrawGizmosSelected();
            }
        }
    }
}
