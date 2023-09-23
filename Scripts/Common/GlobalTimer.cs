using System;
using System.Collections;
using System.Timers;
using UnityEngine;

namespace mj.gist
{
    public class GlobalTimer : MonoBehaviour
    {
        [SerializeField] private int targetFrameRate = 30;
        [SerializeField] private int debugFontSize = 30;
        [SerializeField] private KeyCode showKey = KeyCode.T;
        private bool show = true;

        private float timer;
        private int fps;

        void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFrameRate;
            StartCoroutine(FpsCounter());
        }

        void Update()
        {
            if (Input.GetKeyDown(showKey))
                show = !show;

            timer = Time.realtimeSinceStartup;
        }


        private void OnGUI()
        {
            if (!show) return;

            GUI.skin.label.fontSize = debugFontSize;
            GUILayout.Label($"Fps:  {fps}");
            GUILayout.Label($"Timer: {TimeUtil.SecondsToHMS((int)timer)}");
            GUILayout.Label($"Time: { DateTime.Now.ToString("MMMM dd HH:mm:ss.ff")}");
        }

        IEnumerator FpsCounter()
        {
            int count = 0;
            float time = 0;
            while (true)
            {
                yield return new WaitForEndOfFrame();
                count++;
                time += Time.deltaTime;
                if (time >= 1) { fps = count; count = 0; time = 0; }
            }
        }
    }
}
