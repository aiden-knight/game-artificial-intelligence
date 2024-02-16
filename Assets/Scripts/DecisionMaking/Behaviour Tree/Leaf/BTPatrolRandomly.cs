using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BTPatrolRandomly : BTNode
{
    string targetPosKey = "TargetPos";
    string cancelSeekKey = "CancelSeek";
    string agentPosKey = "AgentPos";

    Pathfinding_JPS m_JPS;

    public BTPatrolRandomly() : base()
    {
        m_JPS = new Pathfinding_JPS(true, false);
    }

    public override BTState Process()
    {
        Vector2 agentPos = (Vector2)m_Blackboard.GetFromDictionary(agentPosKey);
        if (m_JPS.m_Path.Count == 0)
        {
            Rect size = Grid.m_GridSize;
            float x1 = Random.Range(size.xMin, size.xMax);
            float y1 = Random.Range(size.yMin, size.yMax);

            m_JPS.GeneratePath(Grid.GetNodeClosestWalkableToLocation(agentPos), Grid.GetNodeClosestWalkableToLocation(new Vector2(x1, y1)));
            return BTState.PROCESSING;
        }
        else
        {
            if (m_JPS.m_Path.Count > 0)
            {
                Vector2 closestPoint = m_JPS.GetNextPointOnPath(agentPos);

                RaycastHit2D hit = Physics2D.Raycast(agentPos, closestPoint - agentPos, Maths.Magnitude(closestPoint - agentPos));
                if(hit.collider != null)
                {
                    m_JPS.m_Path.Clear();
                    return BTState.PROCESSING;
                }

                m_Blackboard.AddToDictionary(targetPosKey, closestPoint);
                m_Blackboard.AddToDictionary(cancelSeekKey, false);
                return BTState.SUCCESS;
            }
            else
            {
                m_Blackboard.AddToDictionary(cancelSeekKey, true);
                return BTState.FAILURE;
            }
        }
    }
}
