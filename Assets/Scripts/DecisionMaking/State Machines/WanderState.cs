using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderState : FuzzyState
{
    SteeringBehaviour_Manager m_SteeringBehaviours;
    SteeringBehaviour_Wander m_Wander;

    private void Awake()
    {
        m_SteeringBehaviours = GetComponent<SteeringBehaviour_Manager>();
        if (!m_SteeringBehaviours) Debug.LogError("Object doesn't have a Steering Behaviour Manager attached", this);

        m_Wander = gameObject.AddComponent<SteeringBehaviour_Wander>();
        if (!m_Wander) Debug.LogError("Object doesn't have a Wander Steering Behaviour attached", this);

        m_Wander.ShowDebug(true);
    }

    public override void Run()
    {
        m_Wander.m_Weight = Mathf.Lerp(5, 10, m_ActivationDegree);
    }

    public override void Enter()
    {
        m_SteeringBehaviours.AddBehaviour(m_Wander);
    }
    public override void Exit()
    {
        m_SteeringBehaviours.RemoveBehaviour(m_Wander);
    }

    public override float CalculateActivation()
    {
        return m_ActivationDegree = 0.5f;
    }
}
