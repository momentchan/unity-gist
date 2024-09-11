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

    public static Vector3 Invert(this Vector3 vec)
    {
        return new Vector3(1 / vec.x, 1 / vec.y, 1 / vec.z);
    }

    public static float Interpolate(this Vector2 range, float value)
    {
        return (value - range.x) / (range.y - range.x);
    }
    public static float Remap(float value, Vector2 rangeIn, Vector2 rangeOut, bool clamp = true)
    {
        return Remap(value, rangeIn.x, rangeIn.y, rangeOut.x, rangeOut.y, clamp);
    }
    public static float Remap(float value, float in1, float in2, float out1, float out2, bool clamp = true)
    {
        var o = out1 + (value - in1) / (in2 - in1) * (out2 - out1);
        return clamp ? Mathf.Clamp(o, out1, out2) : o;
    }
    public static float Min(this Vector2 vec)
    {
        return Mathf.Min(vec.x, vec.y);
    }
    public static float Min(this Vector3 vec)
    {
        return Mathf.Min(vec.x, vec.y, vec.z);
    }
    public static float Max(this Vector2 vec)
    {
        return Mathf.Max(vec.x, vec.y);
    }
    public static float Max(this Vector3 vec)
    {
        return Mathf.Max(vec.x, vec.y, vec.z);
    }

    public static Vector3 Normalize(this Vector3 vec, float length)
    {
        return vec.normalized * length;
    }
}
