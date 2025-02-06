using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DecisionMakingEntity : MovingEntity
{

    public float m_Acceleration;

    public float m_AlertRadius = 5.0f;
    public bool m_CanMoveWhileAttacking;
    List<WeaponImpl> m_Weapons = new List<WeaponImpl>(); 

    public static Action OnPlayerDead;

    SteeringBehaviour_Manager m_SteeringBehaviours;
    SteeringBehaviour_Seek m_Seek;
    SteeringBehaviour_Evade m_Evade;

    BehaviourTree m_BTree;
    string seekBehaviourKey = "SeekBehaviour";
    string evadeBehaviourKey = "EvadeBehaviour";
    string agentPosKey = "AgentPos";
    string healthKey = "Health";
    string entityKey = "Entity";
    string weaponsKey = "WeaponList";
    string currentWeaponKey = "CurrentWeapon";

    protected override void Awake()
    {
        base.Awake();

        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();
        if (!m_SteeringBehaviours) Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);
        m_Seek = GetComponent<SteeringBehaviour_Seek>();
        if (!m_Seek) Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);
        m_Evade = GetComponent<SteeringBehaviour_Evade>();
        if (!m_Evade) Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);

        m_BTree = new BehaviourTree(
            new BTSelector(new List<BTNode>()
            {
                new BTSequence(new List<BTNode>()
                {
                    new BTEnemiesNearby(m_AlertRadius),
                    new BTClosestVisibleEnemy(),
                    new BTKillClosestEnemy()

                }),
                new BTSequence(new List<BTNode>()
                {
                    new BTHealthPickup(),
                    new BTSeekTo()
                }),
                new BTSequence(new List<BTNode>()
                {
                    new BTPatrolRandomly(),
                    new BTSeekTo()
                })
            })
        );
        m_Evade.m_EvadeRadius = 0.75f * m_AlertRadius;

        m_BTree.m_Blackboard.AddToDictionary(seekBehaviourKey, m_Seek);
        m_BTree.m_Blackboard.AddToDictionary(evadeBehaviourKey, m_Evade);
        m_BTree.m_Blackboard.AddToDictionary(agentPosKey, (Vector2)transform.position);
        m_BTree.m_Blackboard.AddToDictionary(healthKey, GetComponent<Health>());
        m_BTree.m_Blackboard.AddToDictionary(entityKey, this);
    }

    private void Start()
    {
        m_Weapons = GetComponentsInChildren<WeaponImpl>(true).ToList();
        foreach(WeaponImpl weapon in m_Weapons)
        {
            if (weapon == m_Weapons.FirstOrDefault()) continue;
            weapon.gameObject.SetActive(false);
        }

        m_BTree.m_Blackboard.AddToDictionary(weaponsKey, m_Weapons);
        m_BTree.m_Blackboard.AddToDictionary(currentWeaponKey, m_Weapons.FirstOrDefault());
    }

    void Update()
    {
        m_BTree.m_Blackboard.AddToDictionary(agentPosKey, (Vector2)transform.position);
        m_BTree.Process();
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
}
