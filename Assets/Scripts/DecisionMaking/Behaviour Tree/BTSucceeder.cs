using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSucceeder : BTNode
{
    public BTSucceeder(BTNode child) : base(child) { }
    public override BTState Process()
    {
        BTState ret = m_Children[m_ActiveChild].Process();
        if (ret == BTState.PROCESSING)
        {
            return ret;
        }
        else
        {
            return BTState.SUCCESS;
        }
    }
}
