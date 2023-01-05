using System;
using UnityEngine;

namespace mj.gist
{
    public class TimeEvent : MonoBehaviour
    {
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
        }
    }

    [Serializable]
    public class TimeData
    {
        [SerializeField] private int hour;
        [SerializeField] private int minute;
        [SerializeField] private int second;

        public int Hour { get { return hour; } set { hour = value; } }
        public int Minute { get { return minute; } set { minute = value; } }
        public int Second { get { return second; } set { second = value; } }

        public TimeData(DateTime time)
        {
            second = time.Second;
            minute = time.Minute;
            hour = time.Hour;
        }
    }

    public class OneTimeEvent
    {
        public DateTime Time { get; private set; }
        private Action action;
        private string name;
        private bool triggered = false;

        public bool Triggered => triggered;
        public bool Active => Time < DateTime.Now;

        public OneTimeEvent(string name, TimeData time, Action action)
        {
            this.name = name;
            this.Time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, time.Hour, time.Minute, time.Second);
            this.action = action;
        }
        public void Trigger()
        {
            action?.Invoke();
            Debug.Log($"{name} is triggered at {DateTime.Now.ToString("MMMM dd HH:mm")}");
            triggered = true;
        }
    }
}