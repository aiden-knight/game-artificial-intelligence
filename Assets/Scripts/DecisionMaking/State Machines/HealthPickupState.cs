using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickupState : FuzzyState
{
    [SerializeField]
    Pathfinding_JPS m_JPS;

    Health m_Health;
    bool m_PickupExists = false;
    Vector2 m_PickupLocation;

    SteeringBehaviour_Manager m_SteeringBehaviours;
    SteeringBehaviour_Seek m_Seek;
    private void Awake()
    {
        PickupManager.OnPickUpSpawned += ReceiveOnPickUpSpawned;
        Pickup.PickUpCollected += ReceiveOnPickUpCollected;

        DecisionMakingEntity.OnPlayerDead += ReceiveOnPlayerDeath;

        m_Health = GetComponent<Health>();
        if(!m_Health) Debug.LogError("Object doesn't have Health attached", this);
        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();
        if (!m_SteeringBehaviours) Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

        m_Seek = gameObject.AddComponent<SteeringBehaviour_Seek>();
        if (!m_Seek) Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);

        m_JPS = new Pathfinding_JPS(true, false);
        m_Seek.ShowDebug(true);
    }

    public override void Run()
    {
        if (m_JPS.m_Path.Count > 0)
        {
            Vector2 closestPoint = m_JPS.GetClosestPointOnPath(transform.position);

            if (Maths.Magnitude(closestPoint - (Vector2)transform.position) < 0.5f)
                closestPoint = m_JPS.GetNextPointOnPath(transform.position);

            m_Seek.m_TargetPosition = closestPoint;
        }
        m_Seek.m_Weight = Mathf.Lerp(0, 30, m_ActivationDegree);
    }

    public override void Enter()
    {
        m_SteeringBehaviours.AddBehaviour(m_Seek);
        m_JPS.GeneratePath(Grid.GetNodeClosestWalkableToLocation(transform.position), Grid.GetNodeClosestWalkableToLocation(m_PickupLocation));
    }
    public override void Exit()
    {
        m_SteeringBehaviours.RemoveBehaviour(m_Seek);
        m_JPS.m_Path.Clear();
    }

    public override float CalculateActivation()
    {
        if(m_PickupExists)
        {
            return m_ActivationDegree = 1 - m_Health.HealthRatio;
        }
        else
        {
            return m_ActivationDegree = 0f;
        }
    }

    void ReceiveOnPickUpSpawned(Vector3 pHealth, Vector3 pAmmo)
    {
        m_PickupLocation = pHealth;
        m_PickupExists = true;
    }

    void ReceiveOnPickUpCollected()
    {
        m_Seek.m_TargetPosition = Vector2.zero;
        m_PickupExists = false;
    }
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            if (m_JPS.m_Path.Count > 1)
            {
                for (int i = 0; i < m_JPS.m_Path.Count - 1; ++i)
                {
                    Gizmos.DrawLine(m_JPS.m_Path[i], m_JPS.m_Path[i + 1]);
                }
            }
        }
    }

    void ReceiveOnPlayerDeath()
    {
        PickupManager.OnPickUpSpawned -= ReceiveOnPickUpSpawned;
        Pickup.PickUpCollected -= ReceiveOnPickUpCollected;
    }
}
