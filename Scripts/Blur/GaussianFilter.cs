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

        private RenderTexture tmp;
        public int nIterations = 3;
        public int lod = 1;
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
            tmp = DownSample(src, lod, mat);
            Blur(src, dst, nIterations, mat);
            RenderTexture.ReleaseTemporary(tmp);
        }

        public static void Blur(Texture src, RenderTexture dst, int nIterations, Material gaussianMat)
        {
            var tmp0 = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.ARGB32);
            var tmp1 = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.ARGB32);
            var iters = Mathf.Clamp(nIterations, 0, 10);
            Graphics.Blit(src, tmp0);
            for (var i = 0; i < iters; i++)
            {
                for (var pass = 1; pass < 3; pass++)
                {
                    tmp1.DiscardContents();
                    tmp0.filterMode = FilterMode.Bilinear;
                    Graphics.Blit(tmp0, tmp1, gaussianMat, pass);
                    var tmpSwap = tmp0;
                    tmp0 = tmp1;
                    tmp1 = tmpSwap;
                }
            }
            Graphics.Blit(tmp0, dst);
            RenderTexture.ReleaseTemporary(tmp0);
            RenderTexture.ReleaseTemporary(tmp1);
        }

        public static RenderTexture DownSample(Texture src, int lod, Material gaussianMat)
        {
            var dst = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.ARGB32);
            src.filterMode = FilterMode.Bilinear;
            Graphics.Blit(src, dst);

            for (var i = 0; i < lod; i++)
            {
                var tmp = RenderTexture.GetTemporary(dst.width >> 1, dst.height >> 1, 0, dst.format);
                dst.filterMode = FilterMode.Bilinear;
                Graphics.Blit(dst, tmp, gaussianMat, 0);
                RenderTexture.ReleaseTemporary(dst);
                dst = tmp;
            }
            return dst;
        }
    }
}