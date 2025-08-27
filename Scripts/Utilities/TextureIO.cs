using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

public static class TextureIO
{
    // --- Load ---
    /// <summary>
    /// Load a local image file into Texture2D (sync).
    /// </summary>
    public static void LoadTextureFromFile(string filePath, Action<Texture2D> onLoaded, bool mipChain = false, bool linear = false)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            Debug.LogError($"[ImageLoader] File not found: {filePath}");
            onLoaded?.Invoke(null);
            return;
        }

        byte[] bytes = File.ReadAllBytes(filePath);
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, mipChain, linear);

        if (!tex.LoadImage(bytes, markNonReadable: false))
        {
            Debug.LogError($"[ImageLoader] Failed to decode image: {filePath}");
            UnityEngine.Object.Destroy(tex);
            onLoaded?.Invoke(null);
            return;
        }

        tex.name = Path.GetFileNameWithoutExtension(filePath);
        onLoaded?.Invoke(tex);
    }

    /// <summary>
    /// Save RT to PNG.
    /// If RT is HDR, we down-convert to 8-bit LDR without tone mapping (not visually ideal).
    /// For "matches screen" look from an HDR RT, prefer SaveHDRToPNGWithACES().
    /// </summary>
    public static void SaveRenderTextureToPNG(RenderTexture rt, string filePath)
    {
        if (rt == null) { Debug.LogError("RT is null"); return; }

        if (GraphicsFormatUtility.IsHDRFormat(rt.graphicsFormat))
        {
            // Simple HDR->LDR conversion without tone mapping (may clip bright areas).
            var ldr = MakeLdrFromHdr(rt);
            try { SaveLdrRtToPng(ldr, filePath); }
            finally { SafeRelease(ldr); }
        }
        else
        {
            SaveLdrRtToPng(rt, filePath);
        }
    }

    // Save an LDR/sRGB RT to PNG using AsyncGPUReadback to avoid hidden color conversions.
    static void SaveLdrRtToPng(RenderTexture ldrRt, string filePath)
    {
        var req = AsyncGPUReadback.Request(ldrRt, 0, TextureFormat.RGBA32);
        req.WaitForCompletion();
        if (req.hasError) { Debug.LogError("GPU Readback failed."); return; }

        var native = req.GetData<byte>();
        var managed = native.ToArray();

        // Preserve the RT's color encoding (sRGB vs UNorm) to avoid double gamma or clipping.
        var fmt = ldrRt.graphicsFormat;
        if (!IsRGBA8(fmt))
        {
            // Force to a standard 8-bit format if RT is in some unusual format.
            fmt = (QualitySettings.activeColorSpace == ColorSpace.Linear)
                ? GraphicsFormat.R8G8B8A8_SRGB
                : GraphicsFormat.R8G8B8A8_UNorm;
        }

        byte[] png = ImageConversion.EncodeArrayToPNG(
            managed, fmt, (uint)ldrRt.width, (uint)ldrRt.height
        );

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllBytes(filePath, png);
#if UNITY_EDITOR
        Debug.Log($"Saved PNG: {filePath}");
#endif
    }

    // Simple HDR -> LDR 8-bit (no tone map, just format conversion).
    // Bright highlights may clip; prefer ACES path above for screen-match.
    static RenderTexture MakeLdrFromHdr(RenderTexture hdrRt)
    {
        var ldrFmt = (QualitySettings.activeColorSpace == ColorSpace.Linear)
            ? GraphicsFormat.R8G8B8A8_SRGB
            : GraphicsFormat.R8G8B8A8_UNorm;

        var ldr = new RenderTexture(hdrRt.width, hdrRt.height, 0)
        {
            graphicsFormat = ldrFmt,
            antiAliasing = 1,
            useMipMap = false,
            name = "RT_8bit_LDR_sRGB"
        };
        ldr.Create();

        // Blit does format conversion; no tone mapping is applied here.
        Graphics.Blit(hdrRt, ldr);
        return ldr;
    }

    static bool IsRGBA8(GraphicsFormat fmt)
    {
        return fmt == GraphicsFormat.R8G8B8A8_UNorm
            || fmt == GraphicsFormat.B8G8R8A8_UNorm
            || fmt == GraphicsFormat.R8G8B8A8_SRGB
            || fmt == GraphicsFormat.B8G8R8A8_SRGB;
    }

    static void SafeRelease(RenderTexture rt)
    {
        if (rt == null) return;
        rt.Release();
#if UNITY_EDITOR
        UnityEngine.Object.DestroyImmediate(rt);
#else
        UnityEngine.Object.Destroy(rt);
#endif
    }
}
