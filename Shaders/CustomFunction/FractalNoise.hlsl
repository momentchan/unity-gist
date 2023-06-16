#include "Assets/Packages/unity-gist/Cginc/Noise.cginc"

void FractalNoise_float(float2 uv, float3 scale, float3 velocity, float threshold, float strength, float t, out float Out)
{
    uv *= scale.xy;
    float3 p = (float3(uv, 0) + t * float3(-1.0 * velocity.x, -1.0 * velocity.y, -1.0 * velocity.z));

    float3 q = p;
    
    float f;
    f = 0.50000 * noise(q);
    q = q * 2.02;
    f += 0.25000 * noise(q);
    q = q * 2.03;
    f += 0.12500 * noise(q);
    q = q * 2.01;
    f += 0.06250 * noise(q);
    q = q * 2.02;
    f += 0.03125 * noise(q);
    f = pow(saturate((f - threshold)) * strength, scale.z);
	
    Out = saturate(f);
}