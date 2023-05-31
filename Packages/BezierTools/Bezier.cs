using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BezierTools
{
    public class Bezier : ScriptableObject
    {
        [SerializeField]
        public BezierData data;

        public Texture2D BakeToTexture(int sampleCount)
        {
            var rt = new Texture2D(sampleCount, 2, TextureFormat.RGBAFloat, false);
            rt.wrapMode = TextureWrapMode.Clamp;

            for (int i = 0; i < sampleCount; i++)
            {
                var nt = (i * sampleCount) % 1;
                var pos = data.PositionNormalizeT(nt);
                var vel = data.VelocityNormalizeT(nt);
                rt.SetPixel(i, 0, new Color(pos.x, pos.y, pos.z, 0));
                rt.SetPixel(i, 1, new Color(vel.x, vel.y, vel.z, 0));
            }
            rt.Apply();

            return rt;
        }
    }
}
