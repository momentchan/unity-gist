using UnityEngine;
using System.Collections;

namespace mj.gist
{
    [ExecuteInEditMode]
    public class GaussianFilter : MonoBehaviour
    {
        [SerializeField] private Texture src;
        [SerializeField] private RenderTexture dst;
        [SerializeField] private Shader shader;


        public int nIterations = 3;
        public int lod = 1;
        public float step = 1.0f;
        private Material mat
        {
            get
            {
                if (_mat == null)
                    _mat = new Material(shader);
                return _mat;
            }
        }
        private Material _mat;

        private void Update()
        {
            var tmp = BlurUtil.DownSample(src, lod, mat);
            BlurUtil.Blur(src, dst, nIterations, step, mat);
            RenderTexture.ReleaseTemporary(tmp);
        }
    }
}