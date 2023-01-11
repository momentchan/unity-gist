using UnityEngine;

public static class CompareUtil 
{
	public static bool IsInside(this Vector2 range, float value)
    {
        return value > range.x && value < range.y;
    }
}