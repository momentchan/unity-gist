using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mj.gist {
    /// <summary>
    /// Convert uv to object space
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class UvToObjectWrapper : MonoBehaviour {
        private Mesh mesh;
        private Vector3[] vertices;
        private int[] triangles;
        private Vector2[] uvs;
        private Vector3[] normals;
        private Triangle2D[] uvTris;

        void Start() {
            mesh = GetComponent<MeshFilter>().sharedMesh;
            Init();
        }

        private void Init() {
            triangles = mesh.triangles;
            vertices = mesh.vertices;
            normals = mesh.normals;
            uvs = mesh.uv;

            var triangleCount = triangles.Length / 3;
            uvTris = new Triangle2D[triangleCount];

            for (var i = 0; i < triangleCount; i++) {
                Vector2 uva, uvb, uvc;
                GetTriangleUvs(i, out uva, out uvb, out uvc);
                uvTris[i] = new Triangle2D(uva, uvb, uvc);
            }
        }

        public bool GetObjectPos(Vector2 uv, out Vector3 pos, out Vector3 normal) {
            for (var i = 0; i < uvTris.Length; i++) {
                var uvTri = uvTris[i];
                float s, t;
                if (uvTri.Solve(uv, out s, out t)) {
                    var r = 1f - (s + t);
                    pos = InterpolationPosition(i, s, t, r);
                    normal = InterpolationNormal(i, s, t, r);
                    return true;
                }
            }

            pos = default(Vector3);
            normal = default(Vector3);
            return false;
        }

        private Vector3 InterpolationPosition(int i, float s, float t, float r) {
            Vector3 va, vb, vc;
            GetTriangleVertices(i, out va, out vb, out vc);
            return r * va + s * vb + t * vc;
        }
        private Vector3 InterpolationNormal(int i, float s, float t, float r) {
            Vector3 na, nb, nc;
            GetTriangleNormals(i, out na, out nb, out nc);
            return r * na + s * nb + t * nc;
        }

        private void GetTriangleUvs(int i, out Vector2 uva, out Vector2 uvb, out Vector2 uvc) {
            var i3 = i * 3;
            uva = uvs[triangles[i3]];
            uvb = uvs[triangles[i3 + 1]];
            uvc = uvs[triangles[i3 + 2]];
        }

        private void GetTriangleVertices(int i, out Vector3 va, out Vector3 vb, out Vector3 vc) {
            var i3 = i * 3;
            va = vertices[triangles[i3]];
            vb = vertices[triangles[i3 + 1]];
            vc = vertices[triangles[i3 + 2]];
        }

        private void GetTriangleNormals(int i, out Vector3 na, out Vector3 nb, out Vector3 nc) {
            var i3 = i * 3;
            na = normals[triangles[i3]];
            nb = normals[triangles[i3 + 1]];
            nc = normals[triangles[i3 + 2]];
        }

        public class Triangle2D {
            public Vector2 a;
            public Vector2 ab;
            public Vector2 ac;
            public float inv_det_abac;
            public bool valid => Mathf.Abs(inv_det_abac) > Mathf.Epsilon;

            public Triangle2D(Vector2 a, Vector2 b, Vector2 c) {
                this.a = a;
                this.ab = b - a;
                this.ac = c - a;

                var det_abac = (ab.x * ac.y - ab.y * ac.x);
                this.inv_det_abac = 1f / det_abac;
            }

            public bool Solve(Vector2 p, out float s, out float t) {
                if (!valid) {
                    s = 0;
                    t = 0;
                    return false;
                }

                var ap = p - a;
                var det_apac = ap.x * ac.y - ap.y * ac.x;
                s = det_apac * inv_det_abac;

                var det_abap = ab.x * ap.y - ab.y * ap.x;
                t = det_abap * inv_det_abac;

                if (s < 0 || t < 0)
                    return false;

                return 1 - (s + t) >= 0;
            }
        }
    }
}