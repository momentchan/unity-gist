using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class EnumUtil
{
    public static T Step<T>(this T value, int offset) where T : Enum
    {
        var intValue = (int)(object)value;
        var length = Enum.GetValues(typeof(T)).Length;

        intValue = (intValue + offset + length) % length;

        return (T)(object)intValue;
    }
}
