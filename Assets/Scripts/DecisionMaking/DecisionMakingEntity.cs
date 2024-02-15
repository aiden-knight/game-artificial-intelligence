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
    SteeringBehaviour_Seek m_Seek;

    BehaviourTree m_BTree;
    string seekBehaviourKey = "SeekBehaviour";
    string agentPosKey = "AgentPos";
    Pointer m_PosPointer;

    protected override void Awake()
    {
        base.Awake();

        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();
        if (!m_SteeringBehaviours) Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);
        m_Seek = GetComponent<SteeringBehaviour_Seek>();
        if (!m_Seek) Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);

        m_BTree = new BehaviourTree(
            new BTSequence(new List<BTNode>()
                {
                    new BTHealthPickup(),
                    new BTSeekTo()
                })
            );


        m_BTree.m_Blackboard.AddToDictionary(seekBehaviourKey, new Pointer(m_Seek));
        m_PosPointer = new Pointer((Vector2)transform.position);
        m_BTree.m_Blackboard.AddToDictionary(agentPosKey, m_PosPointer);
    }

    private void Start()
    {
        m_Weapons = GetComponentsInChildren<WeaponImpl>(true).ToList();
        m_current = m_Weapons.FirstOrDefault();
    }

    void Update()
    {
        m_PosPointer.Var = (Vector2)transform.position;
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
