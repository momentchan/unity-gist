using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace mj.gist
{
    public class TimeEvent : MonoBehaviour
    {
        [SerializeField] private List<OneTimeEvent> eventData;

        public DateTime Now => DateTime.Now;
        public Action<int> OnSecondChanged;
        public Action<int> OnMinuteChanged;
        public Action<int> OnHourChanged;
        private TimeData time;

        private void Start()
        {
            time = new TimeData(Now);

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

            foreach (var e in eventData)
            {
                if (!e.Triggered && e.Active)
                    e.Trigger();
            }
        }
    }

    [Serializable]
    public class TimeData
    {
        [SerializeField] private int hour;
        [SerializeField] private int minute;
        [SerializeField] private int second;
        [SerializeField] private List<DayOfWeek> dayOfWeeks;

        public bool Contain(DayOfWeek dayOfWeek)
            => dayOfWeeks.Count == 0 || dayOfWeeks.Count != 0 && dayOfWeeks.Contains(dayOfWeek);

        public int Hour { get { return hour; } set { hour = value; } }
        public int Minute { get { return minute; } set { minute = value; } }
        public int Second { get { return second; } set { second = value; } }

        public TimeData(DateTime time)
        {
            second = time.Second;
            minute = time.Minute;
            hour = time.Hour;
        }

        public override string ToString() => $"{hour}:{minute}:{second}";
    }

    [Serializable]
    public class OneTimeEvent
    {
        [SerializeField] private string name = "Event";
        [SerializeField] private TimeData time;
        [SerializeField] private bool triggered = false;
        [SerializeField] private UnityEvent action;

        public DateTime Time => new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, time.Hour, time.Minute, time.Second);

        public bool Triggered => triggered;
        public bool Active => Time < DateTime.Now && time.Contain(DateTime.Now.DayOfWeek);

        public void Trigger()
        {
            action?.Invoke();
            Debug.Log($"{name} {time} is triggered at {DateTime.Now.ToString("MMMM dd HH:mm")}");
            triggered = true;
        }
    }
}