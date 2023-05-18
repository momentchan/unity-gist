#include "Assets/Packages/unity-gist/Cginc/fbm.cginc"

void FBM4_float(float2 p, float t, out float Out)
{
	Out = fbm4(p, t);
}

void FBM3_float(float2 p, float t, out float Out)
{
	Out = fbm3(p, t);
}

void FBM2_float(float2 p, float t, out float Out)
{
	Out = fbm2(p, t);
}
