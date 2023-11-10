using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrthographicCameraGizmos : MonoBehaviour
{
    private Camera _camera;
    private new Camera camera
    {
        get
        {
            if (_camera == null)
                _camera = GetComponent<Camera>();
            return _camera;
        }
    }
    private void OnDrawGizmos()
    {
        var h = camera.orthographicSize * 2;
        var w = h * camera.aspect;
        var depth = camera.farClipPlane - camera.nearClipPlane;

        Gizmos.DrawWireCube(camera.transform.position + camera.transform.forward * (camera.nearClipPlane + depth * 0.5f), new Vector3(w, h, depth));
    }
}
