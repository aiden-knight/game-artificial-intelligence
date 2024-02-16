using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTClosestVisibleEnemy : BTNode
{
    string agentPosKey = "AgentPos";
    string nearEnemiesKey = "NearbyEnemies";
    string closestEnemyKey = "ClosestEnemy";

    public override BTState Process()
    {
        Vector2 agentPos = (Vector2)m_Blackboard.GetFromDictionary(agentPosKey);
        List<SimpleEnemy> nearbyEnemies = (List<SimpleEnemy>)m_Blackboard.GetFromDictionary(nearEnemiesKey);

        SimpleEnemy closestEnemy = null;
        float distance = float.MaxValue;
        foreach(SimpleEnemy enemy in nearbyEnemies)
        {
            Vector2 toEnemy = (Vector2)enemy.transform.position - agentPos;
            float distToEnemy = Maths.Magnitude(toEnemy);
            RaycastHit2D hit = Physics2D.Raycast(agentPos,  toEnemy, distToEnemy);
            if(hit.collider == null || hit.collider.gameObject == enemy.gameObject)
            {
                if(distToEnemy < distance)
                {
                    distance = distToEnemy;
                    closestEnemy = enemy;
                }
            }
        }

        if(closestEnemy == null)
        {
            return BTState.FAILURE;
        }
        else
        {
            m_Blackboard.AddToDictionary(closestEnemyKey, closestEnemy);
            return BTState.SUCCESS;
        }
    }
}
