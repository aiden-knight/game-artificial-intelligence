using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

public class BTSeekTo : BTNode
{
    string targetPosKey = "TargetPos";
    string seekBehaviourKey = "SeekBehaviour";
    string agentPosKey = "AgentPos";
    string cancelSeekKey = "CancelSeek";
    string entityKey = "Entity";

    public override BTState Process()
    {
        Vector2 targetPos = (Vector2)m_Blackboard.GetFromDictionary(targetPosKey);
        SteeringBehaviour_Seek seek = (SteeringBehaviour_Seek)m_Blackboard.GetFromDictionary(seekBehaviourKey);
        Vector2 agentPos = (Vector2)m_Blackboard.GetFromDictionary(agentPosKey);
        bool cancelSeek = (bool)m_Blackboard.GetFromDictionary(cancelSeekKey);
        DecisionMakingEntity entity = (DecisionMakingEntity)m_Blackboard.GetFromDictionary(entityKey);

        if(cancelSeek)
        {
            Debug.Log("Seek Cancelled");
            seek.m_Active = false;
            return BTState.FAILURE;
        }

        entity.m_RotatesBasedOnVelocity = true;
        seek.m_Active = true;
        seek.m_TargetPosition = targetPos;

        if ((targetPos - agentPos).magnitude < 0.1f)
        {
            seek.m_Active = false;
            return BTState.SUCCESS;
        }

        return BTState.PROCESSING;
    }
}
