using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraUtil
{
    public static bool IsInsideCameraView(this Camera cam, Vector3 pos, float offset = 0)
    {
        return GetCameraBounds(cam, offset).Contains(pos);
    }
    public static Bounds GetCameraBounds(this Camera cam, float offset = 0)
    {
        var h = 2.0f * cam.orthographicSize;
        var w = h * cam.aspect;

        var center = cam.transform.position + cam.transform.forward * cam.nearClipPlane;
        var size = new Vector3(w + offset, h + offset, cam.farClipPlane - cam.nearClipPlane);
        return new Bounds(center, size);
    }
}
