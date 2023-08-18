using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mj.gist.tracking.Laser
{
    public struct TrackerData
    {
        public uint active;
        public uint isMoving;
        public Vector2 pos;
        public Vector2 dis;
        public Vector2 dir;
        public float lastUpdateTime;
        public float activeRatio;
    }
}
