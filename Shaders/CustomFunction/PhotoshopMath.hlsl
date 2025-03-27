#include "Assets/Packages/unity-gist/Cginc/PhotoshopMath.cginc"


void Blend_float(float3 baseColor, float3 blendColor, float mode, out float3 Out)
{
    Out = BlendColorBurn(baseColor, blendColor);
}

void HSVShift_float(half3 baseColor, half3 shift, out float3 Out)
{
    half3 hsv = rgb2hsv(baseColor);
    hsv = hsv + shift.xyz;
    hsv.yz = saturate(hsv.yz);
    Out = hsv2rgb(hsv);
}