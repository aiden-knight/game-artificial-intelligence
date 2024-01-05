using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Maths
{
    public static float Magnitude(Vector2 a)
    {
        return Mathf.Sqrt(a.x*a.x + a.y * a.y);
    }

    public static Vector2 Normalise(Vector2 a)
    {
        return a / Magnitude(a);
    }

    public static float Dot(Vector2 lhs, Vector2 rhs)
    {
        return (lhs.x * rhs.x) + (lhs.y * rhs.y);
    }

    /// <summary>
    /// Returns the radians of the angle between two vectors
    /// </summary>
    public static float Angle(Vector2 lhs, Vector2 rhs)
    {
        return Mathf.Acos(Dot(lhs, rhs) / (Magnitude(lhs) * Magnitude(rhs)));
    }

    /// <summary>
    /// Translates a vector by X angle in degrees
    /// </summary>
    public static Vector2 RotateVector(Vector2 vector, float angle)
    {
        angle = Mathf.Deg2Rad * angle;
        float sinAngle = Mathf.Sin(angle);
        float cosAngle = Mathf.Cos(angle);
		return new Vector2( vector.x * cosAngle - vector.y * sinAngle,
                            vector.x * sinAngle + vector.y * cosAngle);
	}
}
