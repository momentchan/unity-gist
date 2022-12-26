using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mj.gist {
    public static class GizmosUtil {
        public static void DrawGizmosRect(Vector3 p, float w, float h) {
            var p0 = p;
            var p1 = p + Vector3.right * w;
            var p2 = p + Vector3.right * w - Vector3.up * h;
            var p3 = p - Vector3.up * h;
            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p0);
        }
        public static void DrawCross(Vector3 p, float length)
        {
            var p0 = p + Vector3.up * length * 0.5f;
            var p1 = p - Vector3.up * length * 0.5f;
            var p2 = p + Vector3.right * length * 0.5f;
            var p3 = p - Vector3.right * length * 0.5f;
            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p2, p3);
        }
    }
}