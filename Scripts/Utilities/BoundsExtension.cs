using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoundsExtension
{
    public static Vector3 GetRandomInside(this Bounds bounds, float border = 0)
    {
        return bounds.GetRandomInside(Vector3.one * border);
    }

    public static Vector3 GetRandomInside(this Bounds bounds, Vector3 border)
    {
        var x = Random.Range(bounds.min.x + border.x, bounds.max.x - border.x);
        var y = Random.Range(bounds.min.y + border.y, bounds.max.y - border.y);
        var z = Random.Range(bounds.min.z + border.z, bounds.max.z - border.z);
        return new Vector3(x, y, z);
    }
}
