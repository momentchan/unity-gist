using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mj.gist
{
    public static class TimeUtil
    {
        public static string SecondsToHMS(int seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            return string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds);
        }
    }
}