#ifndef COMPRESS_HIGHLIGHTS_HLSL
#define COMPRESS_HIGHLIGHTS_HLSL

// 若你的專案在 Linear 色域，建議把 useGamma 設 0
// 若輸入是 sRGB（Gamma），把 useGamma 設 1 讓它自動轉換

inline float3 _SRGBToLinear(float3 c) {
    // 近似，避免依賴額外 include（Shader Graph 在不同 RP 可能抓不到）
    return pow(saturate(c), 2.2);
}

inline float3 _LinearToSRGB(float3 c) {
    return pow(max(c, 0.0), 1.0/2.2);
}

// 核心：高亮壓縮（Reinhard+軟膝，亮度比例縮放避免色偏）
float3 CompressHighlights_float(
    float3 InColor,   // 取樣得來的顏色
    float  t0,        // 壓縮啟動下限（建議 0.45）
    float  t1,        // 壓縮啟動上限（建議 0.75）
    float  k,         // 壓縮強度（1.2 ~ 1.6）
    float  useGamma  // 0=輸入已是 Linear；1=輸入是 sRGB/Gamma
)
{
    float3 col = InColor;

    // sRGB -> Linear（用 useGamma 開關）
    if (useGamma > 0.5)
        col = _SRGBToLinear(col);

    // 亮度（Rec.709）
    float Y  = dot(col, float3(0.2126, 0.7152, 0.0722));

    // 軟膝：只在高亮區啟動壓縮
    float w  = smoothstep(t0, t1, Y);

    // Reinhard 壓縮
    float Yc = Y / (1.0 + k * Y);

    // 亮度比縮放，保留色相/飽和度
    float scale = lerp(1.0, Yc / max(Y, 1e-6), w);
    float3 linOut = col * scale;

    // Linear -> sRGB（若一開始做了 gamma->linear，就轉回）
    if (useGamma > 0.5)
        linOut = _LinearToSRGB(linOut);

    return linOut;
}



void Shifting_float(
	UnityTexture2D Texture,
    UnitySamplerState Sampler,
    float2 UV,
    float MaxSpanUV,
    float Strength,
    float Steps,
    float2 Direction,
    float2 TexelSize,
	float t0,
	float t1,
	float k,
	float useGamma,
    out float4 output)	
{
	output=0;


	if (Steps <= 1)
    {
		float4 s = SAMPLE_TEXTURE2D(Texture, Sampler, UV);
		s.rgb = CompressHighlights_float(s.rgb, t0, t1, k, useGamma);
        output += s * Strength;
        return;
    }

    float invN = 1.0 / (Steps - 1);
    for (int i = 0; i < Steps; i++)
    {
        // i: 0..Steps-1 → t: 0..1 → p: [-MaxSpanUV, +MaxSpanUV]（UV 偏移量）
        float t = i * invN;
        float p = lerp(-MaxSpanUV, MaxSpanUV, t);

        float2 offset = Direction * p;

        // 如需能量守恆可改：Strength / Steps
        // 如需中心較強可加入權重（例如 Gaussian in UV）：
        // float sigma = MaxSpanUV * 0.35; float w = exp(-(p*p)/(2.0*sigma*sigma));
        // output += SAMPLE_TEXTURE2D(Texture, Sampler, UV + offset) * (Strength * w);
		float4 s = SAMPLE_TEXTURE2D(Texture, Sampler, UV + offset);
		s.rgb = CompressHighlights_float(s.rgb, t0, t1, k, useGamma	);
        output += s * Strength;
    }

}


#endif // COMPRESS_HIGHLIGHTS_HLSL
