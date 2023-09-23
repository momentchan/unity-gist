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

        public static long ConvertToUnixTimestamp(DateTime dateTime)
        {
            // The Unix timestamp is the number of seconds since January 1, 1970 (UTC)
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = dateTime.ToUniversalTime() - unixEpoch;
            return (long)timeSpan.TotalSeconds;
        }

        public static DateTime ConvertUnixTimestampToDateTime(long unixTimestamp)
        {
            // The Unix timestamp is the number of seconds since January 1, 1970 (UTC)
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return unixEpoch.AddSeconds(unixTimestamp).ToLocalTime();
        }
    }
}