using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTEnemiesNearby : BTNode
{
    string agentPosKey = "AgentPos";
    string nearEnemiesKey = "NearbyEnemies";

    float m_AlertDistance;
    List<SimpleEnemy> m_EnemyList;
    List<SimpleEnemy> m_NearbyEnemies;

    public BTEnemiesNearby(float alertDistance) : base()
    {
        m_AlertDistance = alertDistance;
        m_EnemyList = new List<SimpleEnemy>();
        m_NearbyEnemies = new List<SimpleEnemy>();

        SimpleEnemy.OnEnemyDeath += OnEnemyDeath;
        SimpleEnemy.OnEnemySpawn += OnEnemySpawn;
        DecisionMakingEntity.OnPlayerDead += ReceiveOnPlayerDeath;
    }

    public override BTState Process()
    {
        Vector2 agentPos = (Vector2)m_Blackboard.GetFromDictionary(agentPosKey);
        m_NearbyEnemies.Clear();
        foreach (SimpleEnemy enemy in m_EnemyList)
        {
            if(Maths.Magnitude(agentPos - (Vector2)enemy.transform.position) < m_AlertDistance)
            {
                m_NearbyEnemies.Add(enemy);
            }
        }

        if (m_NearbyEnemies.Count > 0)
        {
            m_Blackboard.AddToDictionary(nearEnemiesKey, m_NearbyEnemies);
            return BTState.SUCCESS;
        }
        else
        {
            return BTState.FAILURE;
        }
    }

    void OnEnemySpawn(SimpleEnemy enemy)
    {
        m_EnemyList.Add(enemy);
    }

    void OnEnemyDeath(SimpleEnemy enemy)
    {
        m_EnemyList.Remove(enemy);
    }

    void ReceiveOnPlayerDeath()
    {
        SimpleEnemy.OnEnemyDeath -= OnEnemyDeath;
        SimpleEnemy.OnEnemySpawn -= OnEnemySpawn;
    }
}