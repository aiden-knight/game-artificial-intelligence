using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecisionMakingEntity : MovingEntity
{

    public float m_Acceleration;

    public bool m_CanMoveWhileAttacking;
    List<WeaponImpl> m_Weapons = new List<WeaponImpl>(); 

    WeaponImpl m_current;

    public static Action OnPlayerDead;

    SteeringBehaviour_Manager m_SteeringBehaviours;

    protected override void Awake()
    {
        base.Awake();

        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();
        if (!m_SteeringBehaviours) Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);
    }

    private void Start()
    {
        m_Weapons = GetComponentsInChildren<WeaponImpl>(true).ToList();
        m_current = m_Weapons.FirstOrDefault();
    }

    void Update()
    {

    }

    private void EquipWeaponOfType(WeaponType type)
    {
        m_current.gameObject.SetActive(false);

        foreach(WeaponImpl w in m_Weapons)
        {
            if(w.GetWeaponType() == type)
            {
                m_current = w;
                m_current.gameObject.SetActive(true);
                break;
            }
        }
    }

    protected override Vector2 GenerateVelocity()
    {
        return m_SteeringBehaviours.GenerateSteeringForce();
    }

	public override void DestroyEntity()
	{
        OnPlayerDead?.Invoke();
		base.DestroyEntity();
	}

    SimpleEnemy CheckForEnemies()
    {
        Collider2D[] entities = Physics2D.OverlapCircleAll(transform.position, 5.0f);

        if (entities.Length == 0) return null;

        for(int i = 0; i < entities.Length; ++i)
        {
            SimpleEnemy simpleEnemy = entities[i].GetComponent<SimpleEnemy>();

            if (simpleEnemy) return simpleEnemy;
        }

        return null;
    }
}
