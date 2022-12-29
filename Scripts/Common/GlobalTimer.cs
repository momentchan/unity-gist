using System.Collections;
using UnityEngine;

namespace mj.gist
{
    public class GlobalTimer : MonoBehaviour
    {
        [SerializeField] private int targetFrameRate = 30;
        [SerializeField] private int debugFontSize = 30;
        [SerializeField] private KeyCode showKey = KeyCode.E;
        
        private bool show = true;

        private float time;
        private int fps;

        void Start()
        {
            Application.targetFrameRate = targetFrameRate;
            StartCoroutine(FpsCounter());
        }

        void Update()
        {
            if (Input.GetKeyDown(showKey))
                show = !show;

            time = Time.realtimeSinceStartup;
        }

        private void OnGUI()
        {
            if (!show) return;

            GUI.skin.label.fontSize = debugFontSize;
            GUILayout.Label($"Time: {time}");
            GUILayout.Label($"Fps:  {fps}");
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
