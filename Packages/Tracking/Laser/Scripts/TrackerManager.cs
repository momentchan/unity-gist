using System;
using System.Linq;
using System.Runtime.InteropServices;
using Osc;
using UnityEngine;

namespace mj.gist.tracking.Laser
{
    public class TrackerManager : SingletonMonoBehaviour<TrackerManager>
    {
        [SerializeField] private int trackerNum = 20;
        [SerializeField] private float distanceThreshold = 0.1f;
        [SerializeField] private bool mouseUpdate = true;

        public GraphicsBuffer TrackerBuffer { get; private set; }
        public TrackerData[] Trackers { get; private set; }
        public TrackerData[] ActiveTrackers => Trackers.Where(t => t.active == 1).ToArray();

        public TrackerData MouseData => Trackers[mouseId];
        public int TotalTrackerNum => trackerNum + 1; // plus : mouse
        public bool MouseUpdate
        {
            get
            {
                return mouseUpdate;
            }
            set
            {
                mouseUpdate = value;
            }
        }

        private int mouseId => TotalTrackerNum - 1;

        private static readonly float RANDOM_SMOOTH_FACTOR = 0.5f;
        private static readonly float ALIVE_DURATION = 0.1f;
        private static readonly float FADE_DURATION = 1f;

        public void OnReceivePoint(OscPort.Capsule c)
        {
            try
            {
                var msg = c.message;
                var pos = new Vector2(Mathf.Clamp01((float)msg.data[0]), Mathf.Clamp01((float)msg.data[1]));
                var isMoving = (uint)Mathf.CeilToInt((float)msg.data[2]);
                UpdateTrackerData(pos, isMoving);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Trackers = new TrackerData[TotalTrackerNum];
            TrackerBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, TotalTrackerNum, Marshal.SizeOf(typeof(TrackerData)));
        }

        private void Update()
        {
            if (mouseUpdate)
                UpdateMouseTrackerData();

            UpdateTrackers();
        }
        private void UpdateMouseTrackerData()
        {
            var pos = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            var data = new TrackerData
            {
                active = 1,
                isMoving = 1,
                pos = pos,
                dis = pos - MouseData.pos,
                dir = (pos - MouseData.pos).normalized,
                lastUpdateTime = Time.time,
                activeRatio = 1
            };
            Trackers[mouseId] = data;
        }

        private void UpdateTrackers()
        {
            for (var i = 0; i < Trackers.Length; i++)
            {
                var d = Trackers[i];
                if (d.lastUpdateTime < Time.time - ALIVE_DURATION)
                {
                    d.active = 0;
                    d.activeRatio = Mathf.Clamp01(d.activeRatio - Time.deltaTime / FADE_DURATION);
                    Trackers[i] = d;
                }
            }
            TrackerBuffer.SetData(Trackers);
        }

        private void UpdateTrackerData(Vector2 pos, uint isMoving)
        {
            var actives = 0;
            var minId = -1;
            var minDist = 1e5;
            for (var i = 0; i < trackerNum; i++)
            {
                var tracker = Trackers[i];
                var dist = (pos - tracker.pos).magnitude;
                if (dist < minDist) { minId = i; minDist = dist; }
                if (tracker.active == 1) { actives = i; }
            }

            if (minDist < distanceThreshold)
            {
                var d = GetUpdatedTracker(minId, pos, isMoving);
                Trackers[minId] = d;
            }
            else
            {
                var newID = (actives + 1) % trackerNum;
                var d = GetUpdatedTracker(newID, pos, isMoving);
                Trackers[newID] = d;
            }
        }

        private TrackerData GetUpdatedTracker(int id, Vector2 pos, uint isMoving)
        {
            var prevPos = Trackers[id].pos;
            var prevRatio = Trackers[id].activeRatio;

            TrackerData d;
            d.active = 1;
            d.pos = Vector2.Lerp(prevPos, pos, RANDOM_SMOOTH_FACTOR);
            d.dis = d.pos - prevPos;
            d.dir = d.dis.normalized;
            d.isMoving = isMoving;
            d.lastUpdateTime = Time.time;
            d.activeRatio = Mathf.Clamp(prevRatio + Time.deltaTime / FADE_DURATION, 0, 1);
            return d;
        }

        private void OnDestroy()
        {
            TrackerBuffer.Dispose();
        }
    }
}