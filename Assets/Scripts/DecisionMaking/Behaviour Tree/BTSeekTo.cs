using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSeekTo : BTNode
{
    string targetPosKey = "TargetPos";
    string seekBehaviourKey = "SeekBehaviour";
    string agentPosKey = "AgentPos";

    Vector2 previousPos = Vector2.negativeInfinity;
    public override BTState Process()
    {
        Vector2 targetPos = (Vector2)m_Blackboard.GetFromDictionary(targetPosKey);
        SteeringBehaviour_Seek seek = (SteeringBehaviour_Seek)m_Blackboard.GetFromDictionary(seekBehaviourKey);
        Vector2 agentPos = (Vector2)m_Blackboard.GetFromDictionary(agentPosKey);

        if (previousPos == agentPos) return BTState.FAILURE;

        seek.m_TargetPosition = targetPos;
        previousPos = agentPos;

        if((targetPos - agentPos).magnitude < 0.1f) return BTState.SUCCESS;

        return BTState.PROCESSING;
    }
}
