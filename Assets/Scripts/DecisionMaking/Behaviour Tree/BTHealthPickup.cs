using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTHealthPickup : BTNode
{
    string targetPosKey = "TargetPos";

    bool m_PickupExists = false;
    Vector2 m_PickupLocation;

    Pointer m_TargetPos = new Pointer(Vector2.zero);

    public BTHealthPickup() : base()
    {
        PickupManager.OnPickUpSpawned += ReceiveOnPickUpSpawned;
        Pickup.PickUpCollected += ReceiveOnPickUpCollected;
    }

    public override void AddBlackBoardRecursive(BTBlackboard blackboard)
    {
        base.AddBlackBoardRecursive(blackboard);

        m_Blackboard.AddToDictionary(targetPosKey, m_TargetPos);
    }

    public override BTState Process()
    {
        if(m_PickupExists)
        {
            m_TargetPos.Var = m_PickupLocation;
            return BTState.SUCCESS;
        }
        else
        {
            return BTState.FAILURE;
        }
    }

    void ReceiveOnPickUpSpawned(Vector3 pHealth, Vector3 pAmmo)
    {
        m_PickupLocation = pHealth;
        m_PickupExists = true;
    }

    void ReceiveOnPickUpCollected()
    {
        m_PickupExists = false;
    }
}
