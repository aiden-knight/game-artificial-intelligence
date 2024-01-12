using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Manager : MonoBehaviour
{
    public MovingEntity m_Entity { get; private set; }
    public float m_MaxForce = 100;
    public float m_RemainingForce;
    public List<SteeringBehaviour> m_SteeringBehaviours;

	private void Awake()
	{
        m_Entity = GetComponent<MovingEntity>();

        if(!m_Entity)
            Debug.LogError("Steering Behaviours only working on type moving entity", this);
    }

	public Vector2 GenerateSteeringForce()
    {
        m_RemainingForce = m_MaxForce;

        Vector2 total = Vector2.zero;
        foreach(SteeringBehaviour behaviour in m_SteeringBehaviours)
        {
            if (behaviour == null || !behaviour.m_Active) continue;

            Vector2 force = behaviour.CalculateForce();
            float amount = Maths.Magnitude(force);

            if(amount > m_RemainingForce)
            {
                force = Maths.Normalise(force) * m_RemainingForce;
                total += force;
                m_RemainingForce = 0.0f;
                break;
            }
            else
            {
                m_RemainingForce -= amount;
                total += force;
            }
        }
        return total;
    }

    public void EnableExclusive(SteeringBehaviour behaviour)
	{
        if(m_SteeringBehaviours.Contains(behaviour))
		{
            foreach(SteeringBehaviour sb in m_SteeringBehaviours)
			{
                sb.m_Active = false;
			}

            behaviour.m_Active = true;
		}
        else
		{
            Debug.Log(behaviour + " doesn't not exist on object", this);
		}
	}
    public void DisableAllSteeringBehaviours()
    {
        foreach (SteeringBehaviour sb in m_SteeringBehaviours)
        {
            sb.m_Active = false;
        }
    }
}
