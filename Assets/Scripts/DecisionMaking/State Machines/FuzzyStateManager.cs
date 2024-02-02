using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzyStateManager : MonoBehaviour
{
    [SerializeField]
    List<FuzzyState> m_States;
    List<FuzzyState> m_ActiveStates;

    private void Awake()
    {
        m_ActiveStates = new List<FuzzyState>();
    }

    public void CalculateActiveStates()
    {
        List<FuzzyState> priorStates = new List<FuzzyState>();
        priorStates.AddRange(m_ActiveStates);

        m_ActiveStates.Clear();

        foreach(FuzzyState state in m_States)
        {
            if (!state.m_Active) continue;

            if(state.CalculateActivation() > 0.0f) // state is active
            {
                m_ActiveStates.Add(state);
                if(priorStates.Contains(state)) // was state already active
                {
                    priorStates.Remove(state); // if was active don't call exit state later
                }
                else
                {
                    state.Enter(); // if not already active enter state
                }
            }
        }

        foreach(FuzzyState exitedState in priorStates)
        {
            exitedState.Exit();
        }
    }

    public void RunActiveStates()
    {
        foreach(FuzzyState state in m_ActiveStates)
        { 
            state.Run();
        }
    }
}
