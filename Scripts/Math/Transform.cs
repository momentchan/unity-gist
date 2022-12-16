using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mj.gist
{
    public static class Transform
    {
        /// <summary>
        /// Compute matrix to convert coordinates from p0~p3 to (0,0), (0,1), (1,1), (1,0)
        /// To do the reverse -> use inverse matrix
        /// </summary>
        /// <returns></returns>
        public static Matrix4x4 ComputeHomography(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            var sx = p0.x - p1.x + p2.x - p3.x;
            var sy = p0.y - p1.y + p2.y - p3.y;

            var dx1 = p1.x - p2.x;
            var dx2 = p3.x - p2.x;
            var dy1 = p1.y - p2.y;
            var dy2 = p3.y - p2.y;

            var z = (dy1 * dx2) - (dx1 * dy2);
            var g = ((sx * dy1) - (sy * dx1)) / z;
            var h = ((sy * dx2) - (sx * dy2)) / z;

            var system = new[]{
                p3.x * g - p0.x + p3.x,
                p1.x * h - p0.x + p1.x,
                p0.x,
                p3.y * g - p0.y + p3.y,
                p1.y * h - p0.y + p1.y,
                p0.y,
                g,
                h,
            };

            var mtx = Matrix4x4.identity;
            mtx.m00 = system[0]; mtx.m01 = system[1]; mtx.m02 = system[2];
            mtx.m10 = system[3]; mtx.m11 = system[4]; mtx.m12 = system[5];
            mtx.m20 = system[6]; mtx.m21 = system[7]; mtx.m22 = 1f;

            return mtx;
        }
    }
}
