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

    Vector2 m_PreviousPos = Vector2.negativeInfinity;
    int m_Counter = 0;
    public override BTState Process()
    {
        Vector2 targetPos = (Vector2)m_Blackboard.GetFromDictionary(targetPosKey).Var;
        SteeringBehaviour_Seek seek = (SteeringBehaviour_Seek)m_Blackboard.GetFromDictionary(seekBehaviourKey).Var;
        Vector2 agentPos = (Vector2)m_Blackboard.GetFromDictionary(agentPosKey).Var;

        if (m_PreviousPos == agentPos)
        {
            m_Counter++;
            if(m_Counter == 5)
            {
                seek.m_Active = false;
                m_Counter = 0;
                return BTState.FAILURE;
            }
        }
        else
        {
            m_Counter = 0;
        }

        seek.m_Active = true;
        seek.m_TargetPosition = targetPos;
        m_PreviousPos = agentPos;

        if ((targetPos - agentPos).magnitude < 1.5f)
        {
            m_Counter = 0;
            seek.m_Active = false;
            m_PreviousPos = Vector2.negativeInfinity;
            return BTState.SUCCESS;
        }

        return BTState.PROCESSING;
    }
}
