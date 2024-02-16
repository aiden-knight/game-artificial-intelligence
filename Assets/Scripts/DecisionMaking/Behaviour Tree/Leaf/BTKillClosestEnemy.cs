using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTKillClosestEnemy : BTNode
{
    string closestEnemyKey = "ClosestEnemy";
    string entityKey = "Entity";

    public override BTState Process()
    {
        SimpleEnemy enemy = (SimpleEnemy)m_Blackboard.GetFromDictionary(closestEnemyKey);
        DecisionMakingEntity entity = (DecisionMakingEntity)m_Blackboard.GetFromDictionary(entityKey);

        if (!enemy)
        {
            return BTState.SUCCESS;
        }

        entity.m_RotatesBasedOnVelocity = false;
        Vector2 toEnemy = enemy.transform.position - entity.transform.position;
        entity.transform.up = -toEnemy;
        entity.m_current.PullTrigger();

        return BTState.PROCESSING;
    }
}