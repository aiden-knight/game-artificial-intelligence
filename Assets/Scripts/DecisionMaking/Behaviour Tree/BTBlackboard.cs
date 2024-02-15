using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer
{
    public object Var;
    public Pointer(object var)
    {
        Var = var;
    }
}

public class BTBlackboard
{
    protected Dictionary<string, Pointer> m_Info;

    public Pointer GetFromDictionary(string key)
    {
        Pointer ret;
        m_Info.TryGetValue(key, out ret);
        return ret;
    }

    public void AddToDictionary(string key, Pointer value)
    {
        m_Info.Add(key, value);
    }

    public BTBlackboard()
    {
        m_Info = new Dictionary<string, Pointer>();
    }
}
