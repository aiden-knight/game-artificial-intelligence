using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FuzzyState : MonoBehaviour
{
    public bool m_Active = true;
    protected float m_ActivationDegree;

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Run();
    public abstract float CalculateActivation();
}
