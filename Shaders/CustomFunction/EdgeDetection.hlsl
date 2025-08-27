// Sobel Edge Detection (accumulate into output)
void SobelEdgeAccum_float(
    UnityTexture2D Texture,
    UnitySamplerState Sampler,
    float2 UV,
    float2 TexelSize, // 1.0 / float2(width, height)
    float Threshold, // 起始閾值（0~1）
    float Softness, // 閾值過渡寬度（建議 0.01~0.2）
    float Gain, // 邊緣增益（放大梯度，建議 1~4）
    float Strength, // 疊加強度
    out float4 output)   // 累加輸出
{
    output = 0;

    float2 dx = float2(TexelSize.x, 0);
    float2 dy = float2(0, TexelSize.y);

    // 取亮度
    float l00 = dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV - dx - dy).rgb, float3(0.2126, 0.7152, 0.0722));
    float l10 = dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV - dy).rgb, float3(0.2126, 0.7152, 0.0722));
    float l20 = dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV + dx - dy).rgb, float3(0.2126, 0.7152, 0.0722));

    float l01 = dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV - dx).rgb, float3(0.2126, 0.7152, 0.0722));
    float l11 = dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV).rgb, float3(0.2126, 0.7152, 0.0722));
    float l21 = dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV + dx).rgb, float3(0.2126, 0.7152, 0.0722));

    float l02 = dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV - dx + dy).rgb, float3(0.2126, 0.7152, 0.0722));
    float l12 = dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV + dy).rgb, float3(0.2126, 0.7152, 0.0722));
    float l22 = dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV + dx + dy).rgb, float3(0.2126, 0.7152, 0.0722));

    // Scharr 核計算梯度（比 Sobel 平滑且方向一致性更好）
    float gx = (-3 * l00 + 3 * l20) + (-10 * l01 + 10 * l21) + (-3 * l02 + 3 * l22);
    float gy = (-3 * l00 - 10 * l10 - 3 * l20) + (3 * l02 + 10 * l12 + 3 * l22);

    float mag = sqrt(gx * gx + gy * gy) * Gain;

    // 梯度方向量化為四個象限 (0°, 45°, 90°, 135°)
    float angle = degrees(atan2(gy, gx));
    angle = (angle < 0) ? angle + 180 : angle;
    int dir = ((int) round(angle / 45.0)) % 4;

    // 方向上的鄰居
    float mag1 = 0, mag2 = 0;
    if (dir == 0)
    { // 水平
        mag1 = length(float2(gx, gy)); // 當前點已是 mag
        mag2 = max(dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV - dx).rgb, float3(0.2126, 0.7152, 0.0722)),
                   dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV + dx).rgb, float3(0.2126, 0.7152, 0.0722)));
    }
    else if (dir == 1)
    { // 45°
        mag2 = max(dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV - dx - dy).rgb, float3(0.2126, 0.7152, 0.0722)),
                   dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV + dx + dy).rgb, float3(0.2126, 0.7152, 0.0722)));
    }
    else if (dir == 2)
    { // 垂直
        mag2 = max(dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV - dy).rgb, float3(0.2126, 0.7152, 0.0722)),
                   dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV + dy).rgb, float3(0.2126, 0.7152, 0.0722)));
    }
    else
    { // 135°
        mag2 = max(dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV - dx + dy).rgb, float3(0.2126, 0.7152, 0.0722)),
                   dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV + dx - dy).rgb, float3(0.2126, 0.7152, 0.0722)));
    }

    // NMS：非最大則壓為 0
    if (mag < mag2)
        mag = 0;

    // 閾值軟過渡
    float edge = smoothstep(Threshold, Threshold + Softness, mag);

    output += float4(edge, edge, edge, edge) * Strength;
}
inline float Luma(float3 c)
{
    return dot(c, float3(0.2126, 0.7152, 0.0722));
}
void CLD_DoG_float(
    UnityTexture2D Texture,
    UnitySamplerState Sampler,
    float2 UV,
    float2 TexelSize,
    float sigmaSmall,
    float sigmaLarge,
    float tau,
    float threshold,
    float softness,
    float gain,
    float strength,
    out float4 output)
{
    // --- Sobel 梯度 ---
    float3 lumW = float3(0.2126, 0.7152, 0.0722);
    float2 dx = float2(TexelSize.x, 0);
    float2 dy = float2(0, TexelSize.y);

    float l00 = dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV - dx - dy).rgb, lumW);
    float l10 = dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV - dy).rgb, lumW);
    float l20 = dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV + dx - dy).rgb, lumW);

    float l01 = dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV - dx).rgb, lumW);
    float l21 = dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV + dx).rgb, lumW);

    float l02 = dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV - dx + dy).rgb, lumW);
    float l12 = dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV + dy).rgb, lumW);
    float l22 = dot(SAMPLE_TEXTURE2D(Texture, Sampler, UV + dx + dy).rgb, lumW);

    float gx = (-1 * l00 + 1 * l20) + (-2 * l01 + 2 * l21) + (-1 * l02 + 1 * l22);
    float gy = (-1 * l00 - 2 * l10 - 1 * l20) + (1 * l02 + 2 * l12 + 1 * l22);

    // 切線方向（梯度垂直方向）
    float2 dir = normalize(float2(-gy, gx));

    // --- 沿切線方向取樣 (DoG) ---
    const int N = 6; // 採樣半徑（可視化品質調整）
    float sumSmall = 0;
    float sumLarge = 0;
    float wSmallSum = 0;
    float wLargeSum = 0;

    for (int i = -N; i <= N; i++)
    {
        float2 offset = dir * (float) i * TexelSize;
        float lum = Luma(SAMPLE_TEXTURE2D(Texture, Sampler, UV + offset).rgb);

        float wS = exp(-(i * i) / (2.0 * sigmaSmall * sigmaSmall));
        float wL = exp(-(i * i) / (2.0 * sigmaLarge * sigmaLarge));

        sumSmall += lum * wS;
        sumLarge += lum * wL;
        wSmallSum += wS;
        wLargeSum += wL;
    }
    sumSmall /= wSmallSum;
    sumLarge /= wLargeSum;

    // DoG 計算
    float dog = (sumSmall - tau * sumLarge) * gain;

    // 邊緣閾值（軟過渡）
    float edge = smoothstep(threshold, threshold + softness, dog);

    // 輸出黑線白底
    float3 lineCol = 1.0 - edge.xxx;
    output = float4(lineCol, 1.0) * strength;
}