using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mj.gist
{
    public class TimeEvent : MonoBehaviour
    {
        public DateTime Now => DateTime.Now;
        public Action<int> OnSecondChanged;
        public Action<int> OnMinuteChanged;
        public Action<int> OnHourChanged;

        private Time time;

        private void Start()
        {
            time = new Time(Now);

            //OnSecondChanged += (second) => { Debug.Log($"Second: {second}"); };
            //OnMinuteChanged += (minute) => { Debug.Log($"Minute: {minute}"); };
            //OnHourChanged += (hour) => { Debug.Log($"Hour: {hour}"); };
        }

        void Update()
        {
            if (time.Second != Now.Second)
            {
                time.Second = Now.Second;
                OnSecondChanged?.Invoke(time.Second);
            }

            if (time.Minute != Now.Minute)
            {
                time.Minute = Now.Minute;
                OnMinuteChanged?.Invoke(time.Minute);
            }

            if (time.Hour != Now.Hour)
            {
                time.Hour = Now.Hour;
                OnHourChanged?.Invoke(time.Hour);
            }
        }


        public class Time
        {
            public int Second { get; set; }
            public int Minute { get; set; }
            public int Hour { get; set; }
            public Time(DateTime time)
            {
                Second = time.Second;
                Minute = time.Minute;
                Hour = time.Hour;
            }
        }
    }
}