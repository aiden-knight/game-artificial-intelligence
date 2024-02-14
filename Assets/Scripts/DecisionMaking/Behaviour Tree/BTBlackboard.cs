using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTBlackboard
{
    protected Dictionary<string, object> m_Info;

    public object GetFromDictionary(string key)
    {
        object ret = null;
        m_Info.TryGetValue(key, out ret);
        return ret;
    }

    public void AddToDictionary(string key, object value)
    {
        m_Info.Add(key, value);
    }

    public BTBlackboard()
    {
        m_Info = new Dictionary<string, object>();
    }
}
