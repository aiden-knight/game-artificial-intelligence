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

    public WeaponImpl m_current;

    public static Action OnPlayerDead;

    SteeringBehaviour_Manager m_SteeringBehaviours;
    SteeringBehaviour_Seek m_Seek;

    BehaviourTree m_BTree;
    string seekBehaviourKey = "SeekBehaviour";
    string agentPosKey = "AgentPos";
    string healthKey = "Health";
    string entityKey = "Entity";

    protected override void Awake()
    {
        base.Awake();

        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();
        if (!m_SteeringBehaviours) Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);
        m_Seek = GetComponent<SteeringBehaviour_Seek>();
        if (!m_Seek) Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);

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


        m_BTree.m_Blackboard.AddToDictionary(seekBehaviourKey, m_Seek);
        m_BTree.m_Blackboard.AddToDictionary(agentPosKey, (Vector2)transform.position);
        m_BTree.m_Blackboard.AddToDictionary(healthKey, GetComponent<Health>());
        m_BTree.m_Blackboard.AddToDictionary(entityKey, this);
    }

    private void Start()
    {
        m_Weapons = GetComponentsInChildren<WeaponImpl>(true).ToList();
        m_current = m_Weapons.FirstOrDefault();
    }

    void Update()
    {
        m_BTree.m_Blackboard.AddToDictionary(agentPosKey, (Vector2)transform.position);
        m_BTree.Process();
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
}
