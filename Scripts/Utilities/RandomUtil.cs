using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomUtil
{
    public static void RandomState(System.Action dlgt, int seed = 123456789)
    {
        var cache = Random.state;
        Random.InitState(seed);
        dlgt();
        Random.state = cache;
    }
}
