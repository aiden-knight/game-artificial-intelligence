using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTInverter : BTNode
{
    public override BTState Process()
    {
        BTState ret = m_Children[m_ActiveChild].Process();
        if(ret == BTState.SUCCESS)
        {
            return BTState.FAILURE;
        }
        else if(ret == BTState.FAILURE)
        {
            return BTState.SUCCESS;
        }
        return ret;
    }
}
