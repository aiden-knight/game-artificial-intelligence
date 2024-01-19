using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Cohesion : SteeringBehaviour
{
    public float m_CohesionRange;
    
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
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_CohesionRange, agentMask);
        if (entities.Length == 0) return Vector2.zero;

        Vector2 direction = Maths.Normalise(m_Manager.m_Entity.m_Velocity);
        if (direction == Vector2.zero)
        {
            direction = Vector2.up;
        }

        int agentNum = 1;
        Vector2 accumulatedPos = (Vector2) transform.position;
        foreach (Collider2D col in entities)
        {
            if (this.gameObject == col.gameObject) continue;

            Vector2 toEntity = col.transform.position - transform.position;
            float dist = Maths.Magnitude(toEntity);
            float dotProduct = Maths.Dot(direction, toEntity);
            dotProduct /= Maths.Magnitude(direction) * Maths.Magnitude(toEntity);

            // if outside FOV continue
            if (dotProduct < m_FOV) continue;

            accumulatedPos += (Vector2)col.transform.position;
            agentNum++;
        }

        if(agentNum == 1) return Vector2.zero;

        Vector2 cohesionPos = accumulatedPos / agentNum;
        Vector2 dir = Maths.Normalise(cohesionPos - (Vector2)(transform.position));
        m_DesiredVelocity = dir * m_Manager.m_Entity.m_MaxSpeed;

        return m_Weight * (m_DesiredVelocity - m_Manager.m_Entity.m_Velocity);
    }
}
