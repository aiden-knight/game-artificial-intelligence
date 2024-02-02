using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Action OnHealthDepleted;
    public Action<int, int> OnHealthChanged;

    [SerializeField]
    int m_MaxHealth;
    int m_CurrentHealth;

    public float HealthRatio => (float)m_CurrentHealth / m_MaxHealth;

    public void Start()
    {
        m_CurrentHealth = m_MaxHealth;
    }

    public void ApplyDamage(int damage)
    {
        m_CurrentHealth -= damage;
        OnHealthChanged?.Invoke(m_MaxHealth, m_CurrentHealth);
        if(m_CurrentHealth <=0)
        {
            OnHealthDepleted.Invoke();
        }
    }
    public void ApplyHealing(int healing)
    {
        m_CurrentHealth += healing;
        if (m_CurrentHealth > m_MaxHealth)
        {
            m_CurrentHealth = m_MaxHealth;
        }
        OnHealthChanged?.Invoke(m_MaxHealth, m_CurrentHealth);
    }
}
