using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour_Seperation : SteeringBehaviour
{
    public float m_SeperationRange;
    Vector2 accumulatedSeperationForce = Vector2.zero;
    
    [Range(1,-1)]
    public float m_FOV;
    public override Vector2 CalculateForce()
    {
        //delete me
        return Vector2.zero;
    }
}
