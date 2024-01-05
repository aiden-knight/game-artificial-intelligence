using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecisionMakingEntity : MovingEntity
{
    public float m_Acceleration;

    //input
    float m_Horizontal;
    float m_Vertical;

    public bool m_CanMoveWhileAttacking;
    List<WeaponImpl> m_Weapons = new List<WeaponImpl>(); 

    WeaponImpl m_current;

    public static Action OnPlayerDead;

    private void Start()
    {
        m_Weapons = GetComponentsInChildren<WeaponImpl>(true).ToList();
        m_current = m_Weapons.FirstOrDefault();
        PickupManager.OnPickUpSpawned += RecieveOnPickUpSpawned;
    }

    void Update()
    {

        //delete me and replace with AI code.
        m_Horizontal = Input.GetAxis("Horizontal");
        m_Vertical = Input.GetAxis("Vertical");

        if (Input.GetMouseButtonDown(0))
        {
            m_current.PullTrigger();
        }
        if(Input.GetKeyDown(KeyCode.F1))
        {
            EquipWeaponOfType(WeaponType.Handgun);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            EquipWeaponOfType(WeaponType.Machinegun);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            EquipWeaponOfType(WeaponType.Shotgun);
        }

        Vector2 worldXY = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 FaceDir = (Vector2)transform.position - worldXY;
        transform.up = FaceDir.normalized;
    }

    private void EquipWeaponOfType(WeaponType type)
    {
        m_current.gameObject.SetActive(false);

        foreach(WeaponImpl w in m_Weapons)
        {
            if(w.GetWeaponType() == type)
            {
                m_current = w;
                m_current.gameObject.SetActive(true);
                break;
            }
        }
    }

    void RecieveOnPickUpSpawned(Vector3 pHealth, Vector3 pAmmo)
    {
       //Apply decision making influence here
    }

    protected override Vector2 GenerateVelocity()
    {
        return new Vector2(m_Horizontal, m_Vertical) * m_Acceleration;
    }

	public override void DestroyEntity()
	{
        OnPlayerDead?.Invoke();
		base.DestroyEntity();
	}
}
