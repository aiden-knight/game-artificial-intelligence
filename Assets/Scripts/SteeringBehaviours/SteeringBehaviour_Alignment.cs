using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Alignment : SteeringBehaviour
{
    public float m_AlignmentRange;
    
    [Range(1,-1)]
    public float m_FOV;

    LayerMask agentMask;

    private void Start()
    {
        agentMask = LayerMask.GetMask("GroundAI");
    }

    public override Vector2 CalculateForce()
    {
        // get all agents in circle around agent
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_AlignmentRange, agentMask);
        if (entities.Length == 0) return Vector2.zero;

        Vector2 direction = Maths.Normalise(m_Manager.m_Entity.m_Velocity);
        if (direction == Vector2.zero)
        {
            direction = Vector2.up;
        }

        int agentNum = 0;
        Vector2 accumulatedHeading = Vector2.zero;
        foreach (Collider2D col in entities)
        {
            if (this.gameObject == col.gameObject) continue;

            Vector2 toEntity = col.transform.position - transform.position;
            float dist = Maths.Magnitude(toEntity);
            float dotProduct = Maths.Dot(direction, toEntity);
            dotProduct /= Maths.Magnitude(direction) * Maths.Magnitude(toEntity);

            // if outside FOV continue
            if (dotProduct < m_FOV) continue;

            Vector2 entityVelocity = col.GetComponent<MovingEntity>().m_Velocity;
            if(entityVelocity == Vector2.zero)
            {
                entityVelocity = col.transform.up;
            }
            accumulatedHeading += Maths.Normalise(entityVelocity);
            agentNum++;
        }

        if(agentNum > 0)
        {
            Vector2 alignmentForce = accumulatedHeading / agentNum;
            m_Steering = Maths.Normalise(alignmentForce) - direction;
            m_DesiredVelocity = m_Steering * m_Manager.m_Entity.m_MaxSpeed;
            return m_Weight * (m_DesiredVelocity - m_Manager.m_Entity.m_Velocity);
        }
        else
        {
            return Vector2.zero;
        }
    }
}
