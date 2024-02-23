using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTClosestVisibleEnemy : BTNode
{
    string agentPosKey = "AgentPos";
    string nearEnemiesKey = "NearbyEnemies";
    string closestEnemyKey = "ClosestEnemy";
    string evadeBehaviourKey = "EvadeBehaviour";
    string weaponsKey = "WeaponList";
    string currentWeaponKey = "CurrentWeapon";

    public override BTState Process()
    {
        Vector2 agentPos = (Vector2)m_Blackboard.GetFromDictionary(agentPosKey);
        List<SimpleEnemy> nearbyEnemies = (List<SimpleEnemy>)m_Blackboard.GetFromDictionary(nearEnemiesKey);

        SimpleEnemy closestEnemy = null;
        float distance = float.MaxValue;
        int enemyCount = 0;
        List<SimpleEnemy> visibleEnemies = new List<SimpleEnemy>();
        foreach(SimpleEnemy enemy in nearbyEnemies)
        {
            if (enemy == null) continue;

            Vector2 toEnemy = (Vector2)enemy.transform.position - agentPos;
            float distToEnemy = Maths.Magnitude(toEnemy);
            RaycastHit2D hit = Physics2D.Raycast(agentPos,  toEnemy, distToEnemy);
            if(hit.collider == null || hit.collider.gameObject == enemy.gameObject)
            {
                enemyCount++;
                visibleEnemies.Add(enemy);

                if (distToEnemy < distance)
                {
                    distance = distToEnemy;
                    closestEnemy = enemy;
                }
            }
        }

        SteeringBehaviour_Evade evade = (SteeringBehaviour_Evade)m_Blackboard.GetFromDictionary(evadeBehaviourKey);
        if(closestEnemy == null)
        {
            evade.m_Active = false;
            return BTState.FAILURE;
        }
        else
        {
            evade.m_EvadingEntity = closestEnemy;
            evade.m_Active = true;
            m_Blackboard.AddToDictionary(closestEnemyKey, closestEnemy);

            bool defaultWeapon = true;
            List<WeaponImpl> weapons = (List<WeaponImpl>)m_Blackboard.GetFromDictionary(weaponsKey);
            if(enemyCount > 1)
            {
                int closeEnemies = 0;
                foreach(SimpleEnemy enemy in visibleEnemies)
                {
                    if (enemy == closestEnemy) continue;

                    Vector2 toClosestEnemy = (Vector2)closestEnemy.transform.position - agentPos;
                    Vector2 toEnemy = (Vector2)enemy.transform.position - agentPos;


                    float dotProduct = Maths.Dot(toClosestEnemy, toEnemy);
                    dotProduct /= Maths.Magnitude(toEnemy) * Maths.Magnitude(toClosestEnemy);

                    if(dotProduct > 0.7f)
                    {
                        closeEnemies++;
                    }
                }

                if (closeEnemies > 0)
                {
                    int ammo = GetWeaponAmmo(WeaponType.Shotgun, weapons);
                    if (ammo > 0)
                    {
                        defaultWeapon = false;
                        EquipWeaponOfType(WeaponType.Shotgun, weapons);
                    }
                    else
                    {
                        ammo = GetWeaponAmmo(WeaponType.Machinegun, weapons);
                        if (ammo > 0)
                        {
                            defaultWeapon = false;
                            EquipWeaponOfType(WeaponType.Machinegun, weapons);
                        }
                    }
                }
                else
                {
                    int ammo = GetWeaponAmmo(WeaponType.Machinegun, weapons);
                    if (ammo > 0)
                    {
                        defaultWeapon = false;
                        EquipWeaponOfType(WeaponType.Machinegun, weapons);
                    }
                }
            }

            if(defaultWeapon)
            {
                EquipWeaponOfType(WeaponType.Handgun, weapons);
            }

            return BTState.SUCCESS;
        }
    }

    private int GetWeaponAmmo(WeaponType type, List<WeaponImpl> weapons)
    {
        foreach (WeaponImpl w in weapons)
        {
            if (w.GetWeaponType() == type)
            {
                return w.GetAmmoCount();
            }
        }
        return 0;
    }

    private void EquipWeaponOfType(WeaponType type, List<WeaponImpl> weapons)
    {
        WeaponImpl current = (WeaponImpl)m_Blackboard.GetFromDictionary(currentWeaponKey);
        current.gameObject.SetActive(false);

        foreach (WeaponImpl w in weapons)
        {
            if (w.GetWeaponType() == type)
            {
                current = w;
                current.gameObject.SetActive(true);
                m_Blackboard.UpdateDictionary(currentWeaponKey, current);
                break;
            }
        }
    }
}
