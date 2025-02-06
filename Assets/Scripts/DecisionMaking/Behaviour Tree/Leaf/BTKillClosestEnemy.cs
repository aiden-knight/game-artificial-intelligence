using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTKillClosestEnemy : BTNode
{
    string closestEnemyKey = "ClosestEnemy";
    string entityKey = "Entity";
    string currentWeaponKey = "CurrentWeapon";

    public override BTState Process()
    {
        SimpleEnemy enemy = (SimpleEnemy)m_Blackboard.GetFromDictionary(closestEnemyKey);

        if(enemy == null)
        {
            return BTState.FAILURE;
        }

        DecisionMakingEntity entity = (DecisionMakingEntity)m_Blackboard.GetFromDictionary(entityKey);
        WeaponImpl currentWeapon = (WeaponImpl)m_Blackboard.GetFromDictionary(currentWeaponKey);

        entity.m_RotatesBasedOnVelocity = false;
        Vector2 toEnemy = enemy.transform.position - entity.transform.position;
        entity.transform.up = -toEnemy;
        currentWeapon.PullTrigger();
        return BTState.SUCCESS;
    }
}