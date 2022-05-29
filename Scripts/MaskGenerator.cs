using UnityEngine;

namespace mj.gist {
    public class MaskGenerator : MonoBehaviour {
        [SerializeField] private MaskType maskType;
        [SerializeField] private Shader maskShader;
        [SerializeField] private RenderTexture mask;
        [SerializeField] private PingPongRenderTexture rt;
        [SerializeField] private float radius = 0.01f;
        [SerializeField] private float fadeFactor = 0.01f;
        [SerializeField] private float smoothness = 0.01f;

        private Material maskMat;

        private void Start() {
            maskMat = new Material(maskShader);
            rt = new PingPongRenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
        }

        void Update() {
            var mousePose = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            maskMat.SetTexture("_MaskTex", rt.Read);

            maskMat.SetFloat("_Radius", radius);
            maskMat.SetFloat("_Smoothness", smoothness);
            maskMat.SetFloat("_FadeFactor", fadeFactor);

            maskMat.SetFloat("_Aspect", Camera.main.aspect);
            maskMat.SetVector("_Position", mousePose);

            Graphics.Blit(Texture2D.blackTexture, rt.Write, maskMat, (int)maskType);
            rt.Swap();

            Graphics.Blit(rt.Read, mask);
        }

        private void OnDestroy() {
            mask.Release();
            rt.Dispose();
        }

        public enum MaskType { SingleFrame, Accumulation }
    }
}