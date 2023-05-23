using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurUtil
{
    public static void BlurWithDownSample(Texture src, RenderTexture dst, int lod, int nIterations, Material gaussianMat)
    {
        var tmp = DownSample(src, lod, gaussianMat);
        Blur(tmp, dst, nIterations, gaussianMat);

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
