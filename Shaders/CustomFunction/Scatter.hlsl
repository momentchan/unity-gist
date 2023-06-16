#include "Assets/Packages/unity-gist/Cginc/Random.cginc"
float noise(float2 co)
{
    float2 seed = float2(sin(co.x), cos(co.y));
    return frac(sin(dot(seed, float2(12.9898, 78.233))) * 43758.5453);
}
void Scatter_float(float2 uv, float radius, out float2 Out)
{
    Out = -radius + float2(noise(uv), noise(uv.yx)) * radius * 2.0;
}