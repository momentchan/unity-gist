#include "Assets/Packages/unity-gist/Cginc/Constant.cginc"



void DirectionalGaussianBlur_float(UnityTexture2D _BlurTex, UnitySamplerState ss,  float2 UV, int samples, float2 dir, float SD, float blurSize, float aspct, out float4 Out) {

    float invSamples = 1.0 / (samples - 1);
    float dispersion = SD * SD;
    float sum;
    float4 col;
    for (int i = 0; i < samples; i++)
    {
        float offset = (i * invSamples - 0.5) * blurSize * aspct/i;
        float2 uv = UV + dir * offset;
        float gauss = pow(E, -pow(offset, 2) / (2 * dispersion)) / sqrt(PI2 * dispersion);
        sum += gauss;
        col += SAMPLE_TEXTURE2D(_BlurTex,ss, uv) * gauss;
    }
    col /= sum;
    Out =  col;
}