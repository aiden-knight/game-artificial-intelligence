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

    public static Action OnEnemyDeath;

    bool stall = false;
    float stallTime = 2.0f;
    float timeStallStart = 0;

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

	}

	private void Update()
    {
        if (stall && Time.time > timeStallStart + stallTime)
        {
            Resume();
        }
        seek.m_TargetPosition = transformOfInterest.position;
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
        OnEnemyDeath.Invoke();

		DecisionMakingEntity.OnPlayerDead -= DestroyEntity;

		base.DestroyEntity();
    }
}
