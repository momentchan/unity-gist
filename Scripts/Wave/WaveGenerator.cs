using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace NTCAM.Idle
{
    public class WaveGenerator : MonoBehaviour
    {
        [SerializeField] private Shader shader;
        [SerializeField] private RenderTexture rt;
        [SerializeField] private float width = 0.1f;
        [SerializeField] private float fadeFactor = 0.1f;
        [SerializeField] private float distortion = 0f;
        [SerializeField] private float duration = 2f;
        [SerializeField] private bool autoWave = false;

        private int currentId = 0;

        private WaveData[] data;
        private Material mat;
        private GraphicsBuffer buffer;

        private readonly int maxWaves = 20;

        public void SetDuration(float duration)=> this.duration = duration;

        private void Start()
        {
            mat = new Material(shader);
            data = new WaveData[maxWaves];

            buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, maxWaves, Marshal.SizeOf(typeof(WaveData)));
            buffer.SetData(data);

            if (autoWave)
                StartCoroutine(AutoWave());
        }

        void Update()
        {
            buffer.SetData(data);
            mat.SetBuffer("_Waves", buffer);
            mat.SetInt("_WaveCount", maxWaves);
            mat.SetFloat("_Width", width);
            mat.SetFloat("_FadeFactor", fadeFactor);
            mat.SetFloat("_Distortion", distortion);
            mat.SetFloat("_T", Time.time);

            Graphics.Blit(Texture2D.blackTexture, rt, mat);

            if (Input.GetMouseButtonDown(1))
                CreateWave();
        }

        public void CreateWave(bool invert = false)
        {
            for (var i = currentId; i < maxWaves; i++)
            {
                if (data[i].active == 0)
                {
                    StartCoroutine(Wave(i, invert));
                    currentId = (currentId + 1 + maxWaves) % maxWaves;
                    break;
                }
            }
        }


        private IEnumerator AutoWave()
        {
            yield return null;
            while (true)
            {
                CreateWave();
                yield return new WaitForSeconds(Random.Range(5f, 10f));
            }
        }

        private IEnumerator Wave(int id, bool invert)
        {
            yield return null;

            data[id].active = 1;
            var t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                data[id].ratio = invert ? 1 - t / duration : t / duration;
                yield return null;
            }

            data[id].active = 0;
            data[id].ratio = 0;
        }


        private void OnDestroy()
        {
            buffer.Dispose();
            buffer = null;
        }

        public struct WaveData
        {
            public uint active;
            public float ratio;
        }
    }
}