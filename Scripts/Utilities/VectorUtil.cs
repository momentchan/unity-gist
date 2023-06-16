using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorUtil
{
    public static float Lerp(this Vector2 range, float value)
    {
        return Mathf.Lerp(range.x, range.y, value);
    }

    public static float GetRandom(this Vector2 range)
    {
        return Random.Range(range.x, range.y);
    }
}
