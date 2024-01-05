using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Alignment : SteeringBehaviour
{
    public float m_AlignmentRange;
    
    [Range(1,-1)]
    public float m_FOV;
    public override Vector2 CalculateForce()
    {
        //delete me
        return Vector2.zero;
    }
}
