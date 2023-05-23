using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomUtil
{
    public static void RandomState(System.Action dlgt, int seed = 123456789)
    {
        var cache = Random.state;
        Random.InitState(seed);
        dlgt();
        Random.state = cache;
    }

    public static float RandomInRange(this Vector2 range, float seed)
    {
        return Mathf.Lerp(range.x, range.y, seed);
    }
}
