using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Arrive : SteeringBehaviour
{
    [Header("Arrive Properties")]
    [Header("Settings")]
    public Vector2 m_TargetPosition;
    public float m_SlowingRadius; 

    [Space(10)]

    [Header("Debugs")]
    [SerializeField]
    protected Color m_Debug_RadiusColour = Color.yellow;
    [SerializeField]
    protected Color m_Debug_TargetColour = Color.cyan;


    public override Vector2 CalculateForce()
    {
        Vector2 toTarget = m_TargetPosition - (Vector2)(transform.position);
        float magnitude = Maths.Magnitude(toTarget);

        if(magnitude < m_SlowingRadius )
        {
            float deceleration = magnitude / m_SlowingRadius;
            m_DesiredVelocity = deceleration * m_Manager.m_Entity.m_MaxSpeed * Maths.Normalise(toTarget);
        }
        else
        {
            m_DesiredVelocity = Maths.Normalise(toTarget) * m_Manager.m_Entity.m_MaxSpeed;
        }

        return m_Weight * (m_DesiredVelocity - m_Manager.m_Entity.m_Velocity);
    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                Gizmos.color = m_Debug_TargetColour;
                Gizmos.DrawSphere(m_TargetPosition, 0.5f);

                Gizmos.color = m_Debug_RadiusColour;
                Gizmos.DrawWireSphere(transform.position, m_SlowingRadius);

                base.OnDrawGizmosSelected();
            }
        }
    }
}
