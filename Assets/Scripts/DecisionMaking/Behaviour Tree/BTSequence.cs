using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSequence : BTNode
{
    public override BTState Process()
    {
        BTState ret = m_Children[m_ActiveChild].Process();

        if(ret == BTState.FAILURE)
        {
            m_ActiveChild = 0;
            return ret;
        }
        else if(ret == BTState.SUCCESS)
        {
            m_ActiveChild++;
            if(m_ActiveChild == m_Children.Count)
            {
                m_ActiveChild = 0;
                return ret;
            }
            return BTState.PROCESSING;
        }

        return ret;
    }
}
