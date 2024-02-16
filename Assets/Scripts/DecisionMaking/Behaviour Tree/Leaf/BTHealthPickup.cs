using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BTHealthPickup : BTNode
{
    string targetPosKey = "TargetPos";
    string cancelSeekKey = "CancelSeek";
    string agentPosKey = "AgentPos";
    string healthKey = "Health";

    Pathfinding_JPS m_JPS;

    bool m_PickupExists = false;
    Vector2 m_PickupLocation;

    public BTHealthPickup() : base()
    {
        PickupManager.OnPickUpSpawned += ReceiveOnPickUpSpawned;
        Pickup.PickUpCollected += ReceiveOnPickUpCollected;

        DecisionMakingEntity.OnPlayerDead += ReceiveOnPlayerDeath;
        m_JPS = new Pathfinding_JPS(true, false);
    }

    public override BTState Process()
    {
        Health health = (Health)m_Blackboard.GetFromDictionary(healthKey);

        if(health.HealthRatio < 1.0f && m_PickupExists)
        {
            Vector2 agentPos = (Vector2)m_Blackboard.GetFromDictionary(agentPosKey);
            if (m_JPS.m_Path.Count > 0)
            {
                Vector2 closestPoint = m_JPS.GetNextPointOnPath(agentPos);

                RaycastHit2D hit = Physics2D.Raycast(agentPos, closestPoint - agentPos, Maths.Magnitude(closestPoint - agentPos));
                if (hit.collider != null)
                {
                    m_JPS.m_Path.Clear();
                    return BTState.PROCESSING;
                }

                m_Blackboard.AddToDictionary(targetPosKey, closestPoint);
                return BTState.SUCCESS;
            }
            else
            {
                m_JPS.GeneratePath(Grid.GetNodeClosestWalkableToLocation(agentPos), Grid.GetNodeClosestWalkableToLocation(m_PickupLocation));
                return BTState.PROCESSING;
            }
        }
        else
        {
            
            return BTState.FAILURE;
        }
    }

    void ReceiveOnPickUpSpawned(Vector3 pHealth, Vector3 pAmmo)
    {
        m_PickupLocation = pHealth;
        m_Blackboard.AddToDictionary(cancelSeekKey, false);

        m_PickupExists = true;
    }

    void ReceiveOnPickUpCollected()
    {
        m_JPS.m_Path.Clear();
        m_Blackboard.AddToDictionary(cancelSeekKey, true);
        m_PickupExists = false;
    }

    void ReceiveOnPlayerDeath()
    {
        PickupManager.OnPickUpSpawned -= ReceiveOnPickUpSpawned;
        Pickup.PickUpCollected -= ReceiveOnPickUpCollected;
    }
}
