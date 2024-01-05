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
        //delete me
        return Vector2.zero;
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
