#include "Assets/Packages/unity-gist/Cginc/Noise.cginc"

void Snoise3D_float(float3 v, out float3 Out) {
	Out = snoise3D(v);
}