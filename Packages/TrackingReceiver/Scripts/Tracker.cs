using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mj.gist.tracking
{
    public class Tracker : MonoBehaviour
    {
        [SerializeField] private TrackerObject objectPrefab;
        public int playerIndex;
        public List<TrackerObject> trackObjects = new List<TrackerObject>();

        public bool IsActive => idleT < TrackingManager.IDLE_TIME;
        private float idleT = Mathf.Infinity;

        public void Setup(int userId)
        {
            this.playerIndex = userId;
        }

        private void Start()
        {
            for (var i = 0; i < TrackingManager.Instance.TargetJointsCount; i++)
            {
                var o = Instantiate(objectPrefab, transform);
                trackObjects.Add(o);
            }
        }

        private void Update()
        {
            idleT += Time.deltaTime;
        }

        public void Activate()
        {
            idleT = 0;
        }
    }

    public enum JointType : int
    {
        Pelvis = 0,
        SpineNaval = 1,
        SpineChest = 2,
        Neck = 3,
        Head = 4,

        ClavicleLeft = 5,
        ShoulderLeft = 6,
        ElbowLeft = 7,
        WristLeft = 8,
        HandLeft = 9,

        ClavicleRight = 10,
        ShoulderRight = 11,
        ElbowRight = 12,
        WristRight = 13,
        HandRight = 14,

        HipLeft = 15,
        KneeLeft = 16,
        AnkleLeft = 17,
        FootLeft = 18,

        HipRight = 19,
        KneeRight = 20,
        AnkleRight = 21,
        FootRight = 22,

        Nose = 23,
        EyeLeft = 24,
        EarLeft = 25,
        EyeRight = 26,
        EarRight = 27,

        HandtipLeft = 28,
        ThumbLeft = 29,
        HandtipRight = 30,
        ThumbRight = 31,

        Count = 32
    }
}