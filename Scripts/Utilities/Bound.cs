using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bound
{
    [SerializeField] private Transform parent;
    [SerializeField] private Bounds localBound;
    public Vector3 Center => parent.position + localBound.center;
    public Vector3 Size => localBound.size;

    public bool Contains(Vector3 pos)
    {
        return localBound.Contains(parent.InverseTransformPoint(pos));
    }

    public void DrawGizmos()
    {
        Gizmos.DrawWireCube(Center, Size);
    }
}
