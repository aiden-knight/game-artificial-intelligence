using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTBlackboard
{
    protected Dictionary<string, object> m_Info;

    public object GetFromDictionary(string key)
    {
        object ret;
        m_Info.TryGetValue(key, out ret);
        return ret;
    }

    public void AddToDictionary(string key, object value)
    {
        if(m_Info.ContainsKey(key))
        {
            m_Info[key] = value;
        }
        else
        {
            m_Info.Add(key, value);
        }
    }
    public void UpdateDictionary(string key, object value)
    {
        m_Info[key] = value;
    }

    public BTBlackboard()
    {
        m_Info = new Dictionary<string, object>();
    }
}
