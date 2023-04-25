using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Math = Unity.Mathematics.math;

namespace mj.gist.tracking
{
    public class TrackerObject : MonoBehaviour
    {
        [SerializeField] private int uniqueId;
        [SerializeField] private Transform world;
        [SerializeField] private Transform projected;
        [SerializeField] private Transform screen;

        private TrackingManager manager => TrackingManager.Instance;

        private List<MeshRenderer> renderers;
        private float idleT = Mathf.Infinity;

        private Vector2 uv;

        public void UpdatePosition(int uniqueId, Vector3 sensorPos)
        {
            this.uniqueId = uniqueId;
            this.idleT = 0;

            var trans = manager.VolumeTrans;

            var wpos = trans.position + Quaternion.AngleAxis(trans.localEulerAngles.y, Vector3.up) * sensorPos;

            var proj = Vector3.ProjectOnPlane(wpos, -trans.forward);

            var projWorldPos = new Vector3(Vector3.Dot(proj, trans.right), Vector3.Dot(proj, trans.up), Mathf.Abs(sensorPos.z));

            var nmlProjPos = Vector3.Scale(projWorldPos - trans.position, manager.InteractiveRange.Invert()) + Vector3.right * 0.5f;

            var remapPos = new Vector3(Math.unlerp(manager.RangeRemapX.x, manager.RangeRemapX.y, nmlProjPos.x),
                                       Math.unlerp(manager.RangeRemapY.x, manager.RangeRemapY.y, nmlProjPos.y),
                                       Math.unlerp(manager.RangeRemapZ.x, manager.RangeRemapZ.y, nmlProjPos.z));
            uv = remapPos;

            world.transform.position = wpos;
            projected.transform.position = trans.right * projWorldPos.x + trans.up * projWorldPos.y;
            screen.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(uv.x, uv.y, 1));
        }

        private void Start()
        {
            renderers = GetComponentsInChildren<MeshRenderer>().ToList();
        }

        private void Update()
        {
            idleT += Time.deltaTime;

            var active = idleT < TrackingManager.IDLE_TIME;
            foreach (var r in renderers)
                r.enabled = active && manager.DebugMode;
        }
    }
}