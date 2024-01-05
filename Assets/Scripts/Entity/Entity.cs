using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected Animator m_Animator;
    protected SpriteRenderer m_Renderer;
    public int m_AttackPower = 2;

    protected virtual void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Renderer = GetComponent<SpriteRenderer>();

        Health health = GetComponent<Health>();
        if(health != null)
            health.OnHealthDepleted += PlayDeathAnimation;
    }

    public virtual void PlayDeathAnimation()
    {
        if (m_Animator)
            m_Animator.SetTrigger("Die");
        else
            DestroyEntity();
    }

    public virtual void DestroyEntity()
    {
        Destroy(gameObject);
    }
}
