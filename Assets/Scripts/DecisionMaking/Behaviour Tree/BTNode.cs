using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum BTState
{
    SUCCESS,
    FAILURE,
    PROCESSING,
}

public abstract class BTNode
{
    protected BTState m_State;
    protected List<BTNode> m_Children;
    protected int m_ActiveChild = 0;
    protected BTBlackboard m_Blackboard;

    public BTNode()
    {

    }

    public BTNode(BTNode child)
    {
        m_Children = new List<BTNode>() { child };
    }

    public abstract BTState Process();

    public BTNode(List<BTNode> children)
    {
        m_Children = children;
    }

    public void AddBlackBoardRecursive(BTBlackboard blackboard)
    {
        m_Blackboard = blackboard;
        foreach (BTNode child in m_Children)
        {
            child.AddBlackBoardRecursive(blackboard);
        }
    }
}
