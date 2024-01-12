using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SteeringBehaviour_CollisionAvoidance : SteeringBehaviour
{
    [System.Serializable]
    public struct Feeler
	{
        [Range(0, 360)]
        public float m_Angle;
        public float m_MaxLength;
        public Color m_Colour;
    }

    public Feeler[] m_Feelers;
    Vector2[] m_FeelerVectors;
    float[] m_FeelersLength;
    
    [SerializeField]
    LayerMask m_FeelerLayerMask;

    private void Start()
    {
        m_FeelersLength = new float[m_Feelers.Length];
        m_FeelerVectors = new Vector2[m_Feelers.Length];
    }

    public override Vector2 CalculateForce()
    {
        UpdateFeelers();

        float nearestDist = float.MaxValue;
        Vector2 fleePos = Vector2.zero;
        float feelerLength = 1.0f;
        for(int i = 0; i < m_Feelers.Length; ++i)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, m_FeelerVectors[i], m_FeelersLength[i], m_FeelerLayerMask.value);

            if(hit.collider != null)
            {
                Vector3 hitPos = hit.collider.transform.position;
                float dist = Maths.Magnitude(hitPos - transform.position);

                if(dist < nearestDist)
                { 
                    fleePos = hitPos;
                    feelerLength = m_FeelersLength[i];
                    nearestDist = dist;
                }
            }
        }

        if(nearestDist < float.MaxValue)
        {
            Vector2 negativeDir = Maths.Normalise((Vector2)transform.position - fleePos);
            m_DesiredVelocity = negativeDir * m_Manager.m_Entity.m_MaxSpeed;

            float newWeight = Mathf.Lerp(m_Weight, 0, nearestDist / feelerLength);
            return newWeight * (m_DesiredVelocity - m_Manager.m_Entity.m_Velocity);
        }
        else
        {
            m_DesiredVelocity = Vector2.zero;
            return Vector2.zero;
        }
    }

    void UpdateFeelers()
    {
        for (int i = 0; i < m_Feelers.Length; ++i)
        {
            m_FeelersLength[i] = Mathf.Lerp(1, m_Feelers[i].m_MaxLength, Maths.Magnitude(m_Manager.m_Entity.m_Velocity) / m_Manager.m_Entity.m_MaxSpeed);
            m_FeelerVectors[i] = Maths.RotateVector(Maths.Normalise(m_Manager.m_Entity.m_Velocity), m_Feelers[i].m_Angle) * m_FeelersLength[i];
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                for (int i = 0; i < m_Feelers.Length; ++i)
                {
                    Gizmos.color = m_Feelers[i].m_Colour;
                    Gizmos.DrawLine(transform.position, (Vector2)transform.position + m_FeelerVectors[i]);
                }

                base.OnDrawGizmosSelected();
            }
        }
    }
}
