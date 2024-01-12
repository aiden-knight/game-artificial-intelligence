using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Seperation : SteeringBehaviour
{
    public float m_SeperationRange;
    Vector2 accumulatedSeperationForce = Vector2.zero;
    
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
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, m_SeperationRange, agentMask);
        if(entities.Length == 0) return Vector2.zero;

        accumulatedSeperationForce = Vector2.zero;
        Vector2 direction = Maths.Normalise(m_Manager.m_Entity.m_Velocity);
        if(direction == Vector2.zero)
        {
            direction = Vector2.up;
        }

        foreach(Collider2D col in entities)
        {
            Vector2 toEntity = col.transform.position - transform.position;
            float dist = Maths.Magnitude(toEntity);
            float dotProduct = Maths.Dot(direction, toEntity);
            dotProduct /= Maths.Magnitude(direction) * Maths.Magnitude(toEntity);

            // if outside FOV continue
            if (dotProduct < m_FOV) continue;

            accumulatedSeperationForce += Maths.Normalise(toEntity) / dist;
        }


        accumulatedSeperationForce = -1 * Maths.Normalise(accumulatedSeperationForce);
        m_DesiredVelocity = accumulatedSeperationForce * m_Manager.m_Entity.m_MaxSpeed;

        return m_Weight * (m_DesiredVelocity - m_Manager.m_Entity.m_Velocity);
    }
}
