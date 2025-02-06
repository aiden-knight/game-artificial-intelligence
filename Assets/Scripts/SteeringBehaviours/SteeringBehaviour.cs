using UnityEngine;

[RequireComponent(typeof(MovingEntity))]
public abstract class SteeringBehaviour : MonoBehaviour
{
    [Header("Base Properties")]

    [Header("Settings")]
    public bool m_Active = true;
    public float m_Weight = 5;
    
    [Space(10)]

    [Header("Debugs")]
    [SerializeField]
    protected bool m_Debug_ShowDebugLines = false;
    [SerializeField]
    protected Color m_Debug_DesiredVelocityColour = Color.blue;
    [SerializeField]
    protected Color m_Debug_CurrentVelocityColour = Color.green;
    [SerializeField]
    protected Color m_Debug_SteeringColour = Color.red;

    [Space(10)]

    protected SteeringBehaviour_Manager m_Manager;
    protected Vector2 m_DesiredVelocity;
    protected Vector2 m_Steering;

    public void ShowDebug(bool debug)
    {
        m_Debug_ShowDebugLines = debug;
    }

    private void Awake()
    {
        m_Manager = GetComponent<SteeringBehaviour_Manager>();

        if (!m_Manager) 
            Debug.LogError("No Steering Behaviour Manager attached to object", this);
    }

    public abstract Vector2 CalculateForce();

    protected virtual void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_Debug_ShowDebugLines && m_Active && m_Manager.m_Entity)
            {
                //desired velocity
                Gizmos.color = m_Debug_DesiredVelocityColour;
                Gizmos.DrawLine(transform.position, (Vector2)transform.position + m_DesiredVelocity);

                //current velocity
                Gizmos.color = m_Debug_CurrentVelocityColour;
                Gizmos.DrawLine(transform.position, (Vector2)transform.position + m_Manager.m_Entity.m_Velocity);

                //steering
                Gizmos.color = m_Debug_SteeringColour;
                Gizmos.DrawLine((Vector2)transform.position + m_Manager.m_Entity.m_Velocity, (Vector2)transform.position + m_Manager.m_Entity.m_Velocity + m_Steering);
            }
        }
    }
}
