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
    string weaponsKey = "WeaponList";

    Pathfinding_AStar m_AStar;

    bool m_PickupExists = false;
    bool m_AmmoPickup = false;
    Vector2 m_PickupLocation;

    public BTHealthPickup() : base()
    {
        PickupManager.OnPickUpSpawned += ReceiveOnPickUpSpawned;
        Pickup.PickUpCollected += ReceiveOnPickUpCollected;

        DecisionMakingEntity.OnPlayerDead += ReceiveOnPlayerDeath;
        m_AStar = new Pathfinding_AStar(true, false);
    }

    public override BTState Process()
    {
        Health health = (Health)m_Blackboard.GetFromDictionary(healthKey);

        if((m_AmmoPickup || health.HealthRatio < 1.0f) && m_PickupExists)
        {
            Vector2 agentPos = (Vector2)m_Blackboard.GetFromDictionary(agentPosKey);
            if (m_AStar.m_Path.Count > 0)
            {
                Vector2 closestPoint = m_AStar.GetNextPointOnPath(agentPos);

                float dist = Maths.Magnitude(closestPoint - agentPos);
                RaycastHit2D hit = Physics2D.Raycast(agentPos, closestPoint - agentPos, dist - 0.5f);
                if (hit.collider != null || dist > 2.0f)
                {
                    m_AStar.m_Path.Clear();
                    return BTState.PROCESSING;
                }

                m_Blackboard.AddToDictionary(targetPosKey, closestPoint);
                return BTState.SUCCESS;
            }
            else
            {
                m_AStar.GeneratePath(Grid.GetNodeClosestWalkableToLocation(agentPos), Grid.GetNodeClosestWalkableToLocation(m_PickupLocation));
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
        m_AmmoPickup = false;

        Health health = (Health)m_Blackboard.GetFromDictionary(healthKey);
        if(health.HealthRatio > 0.5f)
        {
            List<WeaponImpl> weapons = (List<WeaponImpl>)m_Blackboard.GetFromDictionary(weaponsKey);
            foreach (WeaponImpl weapon in weapons)
            {
                if (weapon.GetAmmoCount() == 0)
                {
                    m_PickupLocation = pAmmo;
                    m_AmmoPickup = true;
                    break;
                }
            }
        }

        m_PickupExists = true;
    }

    void ReceiveOnPickUpCollected()
    {
        m_AStar.m_Path.Clear();
        m_Blackboard.AddToDictionary(cancelSeekKey, true);
        m_PickupExists = false;
    }

    void ReceiveOnPlayerDeath()
    {
        PickupManager.OnPickUpSpawned -= ReceiveOnPickUpSpawned;
        Pickup.PickUpCollected -= ReceiveOnPickUpCollected;
    }
}
