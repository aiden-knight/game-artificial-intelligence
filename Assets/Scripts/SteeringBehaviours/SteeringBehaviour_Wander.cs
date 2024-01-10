using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SteeringBehaviour_Wander : SteeringBehaviour
{
    [Header("Wander Properties")]
    [Header("Settings")]
    public float m_WanderRadius = 2; 
    public float m_WanderOffset = 2;
    public float m_AngleDisplacement = 2;

    Vector2 m_CirclePosition;
    Vector2 m_PointOnCircle;
    float m_Angle = 0.0f;

    [Space(10)]

    [Header("Debugs")]
    [SerializeField]
    protected Color m_Debug_RadiusColour = Color.yellow;
    [SerializeField]
    protected Color m_Debug_TargetColour = Color.cyan;


    public override Vector2 CalculateForce()
    {
        Vector2 direction = m_PointOnCircle - (Vector2)transform.position;
        if(Maths.Magnitude(direction) == 0.0f)
        {
            direction = Vector2.up;
        }
        else
        {
            direction = Maths.Normalise(direction);
        }

        m_CirclePosition = (direction * m_WanderOffset);
        float rads = Mathf.Deg2Rad * m_AngleDisplacement;
        m_Angle += Random.Range(-rads, rads);

        float sinAngle = Mathf.Sin(m_Angle);
        float cosAngle = Mathf.Cos(m_Angle);
        m_PointOnCircle = new Vector2(cosAngle - sinAngle, sinAngle + cosAngle);
        m_PointOnCircle = Maths.Normalise(m_PointOnCircle)* m_WanderRadius;
        m_PointOnCircle += m_CirclePosition;

        Vector2 dir = Maths.Normalise(m_PointOnCircle - (Vector2)(transform.position));
        m_DesiredVelocity = dir * m_Manager.m_Entity.m_MaxSpeed;

        return m_Weight * (m_DesiredVelocity - m_Manager.m_Entity.m_Velocity);
    }

	protected override void OnDrawGizmosSelected()
	{
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(m_CirclePosition, m_WanderRadius);

				Gizmos.color = Color.blue;
				Gizmos.DrawLine(transform.position, m_CirclePosition);

				Gizmos.color = Color.green;
				Gizmos.DrawLine(m_CirclePosition, m_PointOnCircle);

				Gizmos.color = Color.red;
				Gizmos.DrawLine(transform.position, m_PointOnCircle);

                base.OnDrawGizmosSelected();
			}
        }
	}
}
