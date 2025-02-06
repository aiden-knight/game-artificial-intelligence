using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree
{
    BTNode m_Root;
    public BTBlackboard m_Blackboard;
    
    public BehaviourTree(BTNode root)
    {
        m_Blackboard = new BTBlackboard();
        m_Root = root;
        m_Root.AddBlackBoardRecursive(m_Blackboard);
    }

    public void Process()
    {
        m_Root.Process();
    }
}
