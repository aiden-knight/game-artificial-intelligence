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
        //delete me
        return Vector2.zero;
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
