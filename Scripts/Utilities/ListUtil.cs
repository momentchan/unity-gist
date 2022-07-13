using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mj.gist {
    public static class ListUtil {
        public static void Iterate<T>(Action<T> cb) where T : Enum {
            foreach (var v in Enum.GetValues(typeof(T))) cb((T)v);
        }
    }
}