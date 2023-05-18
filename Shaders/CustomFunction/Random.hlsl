#include "Assets/Packages/unity-gist/Cginc/Random.cginc"

void Nrand_float(float2 uv, out float Out)
{
    Out = nrand(uv);
}

void Nrand3_float(float2 uv, out float3 Out)
{
    Out = nrand3(uv);
}