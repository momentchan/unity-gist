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

    public static T RandomPick<T>(this List<T> list)
    {
        if (list == null)
        {
            throw new System.ArgumentNullException(nameof(list));
        }

        if (list.Count == 0)
        {
            throw new System.InvalidOperationException("The list is empty.");
        }

        int index = new System.Random().Next(list.Count);
        return list[index];
    }

}
