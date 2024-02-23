using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BTPatrolRandomly : BTNode
{
    string targetPosKey = "TargetPos";
    string cancelSeekKey = "CancelSeek";
    string agentPosKey = "AgentPos";

    Pathfinding_AStar m_AStar;

    public BTPatrolRandomly() : base()
    {
        m_AStar = new Pathfinding_AStar(true, false);
    }

    public override BTState Process()
    {
        Vector2 agentPos = (Vector2)m_Blackboard.GetFromDictionary(agentPosKey);
        if (m_AStar.m_Path.Count == 0)
        {
            Rect size = Grid.m_GridSize;
            float x1 = Random.Range(size.xMin, size.xMax);
            float y1 = Random.Range(size.yMin, size.yMax);

            m_AStar.GeneratePath(Grid.GetNodeClosestWalkableToLocation(agentPos), Grid.GetNodeClosestWalkableToLocation(new Vector2(x1, y1)));
            return BTState.PROCESSING;
        }
        else
        {
            if (m_AStar.m_Path.Count > 0)
            {
                Vector2 closestPoint = m_AStar.GetNextPointOnPath(agentPos);

                float dist = Maths.Magnitude(closestPoint - agentPos);
                RaycastHit2D hit = Physics2D.Raycast(agentPos, closestPoint - agentPos, dist);
                if (hit.collider != null || dist > 2.0f)
                {
                    m_AStar.m_Path.Clear();
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
