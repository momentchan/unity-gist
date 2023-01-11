using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    /// <summary>
    /// Inverts a scale vector by dividing 1 by each component
    /// </summary>
    public static Vector3 Invert(this Vector3 vec)
    {
        return new Vector3(1 / vec.x, 1 / vec.y, 1 / vec.z);
    }

    public static float Interpolate(this Vector2 range, float value)
    {
        return (value - range.x) / (range.y - range.x);
    }
}
