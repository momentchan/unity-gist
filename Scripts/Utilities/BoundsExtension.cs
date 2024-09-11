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

    public static Vector3 GetClosestPointOnBounds(this Bounds bounds, Vector3 p)
    {
        if (!bounds.Contains(p))
        {
            return bounds.ClosestPoint(p);
        }

        var closestPoint = p;

        var min = bounds.min;
        var max = bounds.max;

        float closestX = Mathf.Abs(p.x - min.x) < Mathf.Abs(p.x - max.x) ? min.x : max.x;
        float closestY = Mathf.Abs(p.y - min.y) < Mathf.Abs(p.y - max.y) ? min.y : max.y;
        float closestZ = Mathf.Abs(p.z - min.z) < Mathf.Abs(p.z - max.z) ? min.z : max.z;

        float distanceX = Mathf.Abs(p.x - closestX);
        float distanceY = Mathf.Abs(p.y - closestY);
        float distanceZ = Mathf.Abs(p.z - closestZ);

        if (distanceX <= distanceY && distanceX <= distanceZ)
        {
            closestPoint = new Vector3(closestX, p.y, p.z);
        }
        else if (distanceY <= distanceX && distanceY <= distanceZ)
        {
            closestPoint = new Vector3(p.x, closestY, p.z);
        }
        else
        {
            closestPoint = new Vector3(p.x, p.y, closestZ);
        }

        return closestPoint;
    }
}
