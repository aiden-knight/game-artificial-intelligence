using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class SimpleEnemy : MovingEntity
{
    [SerializeField]
    Transform transformOfInterest;

    SteeringBehaviour_Seek seek;
    SteeringBehaviour_Manager manageer;

    public static Action<SimpleEnemy> OnEnemyDeath;
    public static Action<SimpleEnemy> OnEnemySpawn;

    bool stall = false;
    float stallTime = 2.0f;
    float timeStallStart = 0;

    Pathfinding_JPS m_JPS;

    public void InitTarget(Transform target)
    {
        transformOfInterest = target;
    }

    private void Start()
    {        
        seek = GetComponent<SteeringBehaviour_Seek>();
        manageer = GetComponent<SteeringBehaviour_Manager>();
        GetComponent<ApplyDamage>().OnDamageDealt += Stall;
        DecisionMakingEntity.OnPlayerDead += DestroyEntity;

        m_JPS = new Pathfinding_JPS(true, false);

        OnEnemySpawn.Invoke(this);
	}

	private void Update()
    {
        if (stall && Time.time > timeStallStart + stallTime)
        {
            Resume();
        }

        Vector2 toTarget = transformOfInterest.position - transform.position;
        float dist = Maths.Magnitude(toTarget);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, toTarget, dist + 1.0f);
        if (hit.collider.gameObject != transformOfInterest.gameObject)
        {
            if (m_JPS.m_Path.Count == 0)
            {
                m_JPS.GeneratePath(Grid.GetNodeClosestWalkableToLocation(transform.position), Grid.GetNodeClosestWalkableToLocation(transformOfInterest.position));
            }
            else
            {
                if (m_JPS.m_Path.Count > 0)
                {
                    Vector2 closestPoint = m_JPS.GetClosestPointOnPath(transform.position);

                    if (Maths.Magnitude(closestPoint - (Vector2)transform.position) < 0.5f)
                        closestPoint = m_JPS.GetNextPointOnPath(transform.position);

                    seek.m_TargetPosition = closestPoint;
                }
            }
        }
        else
        {
            m_JPS.m_Path.Clear();
            seek.m_TargetPosition = transformOfInterest.position;
        }
    }
    
    protected override Vector2 GenerateVelocity()
    {
        if (stall) return Vector2.zero;
        return manageer.GenerateSteeringForce();
    }

    private void Resume()
    {
        timeStallStart = Time.time;
        stall = false;
        GetComponent<ApplyDamage>().enabled = true;
    }

    private void Stall()
    {
        timeStallStart = Time.time;
        stall = true;
        GetComponent<ApplyDamage>().enabled = false;
    }

    public override void DestroyEntity()
    {
        OnEnemyDeath.Invoke(this);

		DecisionMakingEntity.OnPlayerDead -= DestroyEntity;

		base.DestroyEntity();
    }
}
