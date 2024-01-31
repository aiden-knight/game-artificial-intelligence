using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecisionMakingEntity : MovingEntity
{
    private enum States
    {
        WANDER,
        EVADE,
        SEEK,
    };

    public float m_Acceleration;

    public bool m_CanMoveWhileAttacking;
    List<WeaponImpl> m_Weapons = new List<WeaponImpl>(); 

    WeaponImpl m_current;

    public static Action OnPlayerDead;

    SteeringBehaviour_Manager m_SteeringBehaviours;
    SteeringBehaviour_Seek m_Seek;
    SteeringBehaviour_Flee m_Flee;
    SteeringBehaviour_Wander m_Wander;

    States m_State;
    bool m_PickupExists = false;

    protected override void Awake()
    {
        base.Awake();

        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();
        if (!m_SteeringBehaviours) Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);
        m_Seek = GetComponent<SteeringBehaviour_Seek>();
        if (!m_Seek) Debug.LogError("Object doesn't have a Seek Steering Behaviour attached", this);
        m_Flee = GetComponent<SteeringBehaviour_Flee>();
        if (!m_Flee) Debug.LogError("Object doesn't have a Flee Steering Behaviour attached", this);
        m_Wander = GetComponent<SteeringBehaviour_Wander>();
        if (!m_Wander) Debug.LogError("Object doesn't have a Wander Steering Behaviour attached", this);
    }

    private void Start()
    {
        m_Weapons = GetComponentsInChildren<WeaponImpl>(true).ToList();
        m_current = m_Weapons.FirstOrDefault();
        PickupManager.OnPickUpSpawned += RecieveOnPickUpSpawned;
        Pickup.PickUpCollected += ReceiveOnPickUpCollected;

        UpdateState(States.WANDER);
    }

    void Update()
    {
        SimpleEnemy closest = CheckForEnemies();

        switch(m_State)
        {
            case States.WANDER:
                if(m_PickupExists)
                {
                    UpdateState(States.SEEK);
                }
                else if(closest)
                {
                    UpdateState(States.EVADE);
                }
                break;
            case States.EVADE:
                m_Flee.m_FleeTarget = closest?.transform;

                if (m_PickupExists)
                {
                    UpdateState(States.SEEK);
                }
                else if (!closest)
                {
                    UpdateState(States.WANDER);
                }
                break;
            case States.SEEK:
                if(!m_PickupExists)
                {
                    if(closest)
                    {
                        UpdateState(States.EVADE);
                    }
                    else
                    {
                        UpdateState(States.WANDER);
                    }
                }
                break;
        }

        if(m_Flee.m_FleeTarget == null)
        {
            m_Flee.m_FleeTarget = CheckForEnemies()?.transform;
        }



        if (Input.GetMouseButtonDown(0))
        {
            m_current.PullTrigger();
        }
        if(Input.GetKeyDown(KeyCode.F1))
        {
            EquipWeaponOfType(WeaponType.Handgun);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            EquipWeaponOfType(WeaponType.Machinegun);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            EquipWeaponOfType(WeaponType.Shotgun);
        }
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

    void RecieveOnPickUpSpawned(Vector3 pHealth, Vector3 pAmmo)
    {
        m_Seek.m_TargetPosition = pAmmo;
        m_PickupExists = true;
    }

    void ReceiveOnPickUpCollected()
    {
        m_Seek.m_TargetPosition = Vector2.zero;
        m_PickupExists = false;
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

    void UpdateState(States NewState)
    {
        m_Seek.m_Active = false;
        m_Flee.m_Active = false;
        m_Wander.m_Active = false;

        m_State = NewState;
        switch(m_State)
        {
            case States.WANDER:
                m_Wander.m_Active = true;
                break;
            case States.EVADE:
                m_Flee.m_Active = true;
                break;
            case States.SEEK:
                m_Seek.m_Active = true;
                break;
            default:
                break;
        }
    }
}
