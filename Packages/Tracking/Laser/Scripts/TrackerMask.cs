using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static mj.gist.MaskGenerator;

namespace mj.gist.tracking.Laser
{
    public class TrackerMask : MonoBehaviour
    {
        public RenderTexture Mask => mask;

        [SerializeField] private float radius = 0.05f;
        [SerializeField] private RenderTexture mask;
        [SerializeField] private Shader shader;

        private Material mat;

        void Awake()
        {
            mat = new Material(shader);
        }

        void Update()
        {
            mat.SetFloat("_Radius", radius);
            mat.SetFloat("_Aspect", Camera.main.aspect);
            mat.SetBuffer("_TrackerBuffer", TrackerManager.Instance.TrackerBuffer);
            mat.SetInt("_TrackerNum", TrackerManager.Instance.TotalTrackerNum);
            Graphics.Blit(Texture2D.blackTexture, mask, mat);
        }
    }
}