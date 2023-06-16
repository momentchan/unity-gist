#include "Assets/Packages/unity-gist/Cginc/PhotoshopMath.cginc"


void Blend_float(float3 baseColor, float3 blendColor, float mode, out float3 Out)
{
    Out = BlendColorBurn(baseColor, blendColor);
}