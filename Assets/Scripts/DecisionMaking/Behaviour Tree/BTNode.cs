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

    protected BTNode() { }
    protected BTNode(BTNode child)
    {
        m_Children = new List<BTNode>() { child };
    }
    protected BTNode(List<BTNode> children)
    {
        m_Children = children;
    }
    public abstract BTState Process();

    public virtual void AddBlackBoardRecursive(BTBlackboard blackboard)
    {
        m_Blackboard = blackboard;
        if (m_Children == null) return;

        foreach (BTNode child in m_Children)
        {
            child.AddBlackBoardRecursive(blackboard);
        }
    }
}
