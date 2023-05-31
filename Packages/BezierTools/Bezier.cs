using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BezierTools
{
    public class Bezier : ScriptableObject
    {
        [SerializeField]
        public BezierData data;
    }
}
