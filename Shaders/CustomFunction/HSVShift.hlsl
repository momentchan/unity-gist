#include "Assets/Packages/unity-gist/Cginc/PhotoShopMath.cginc"

void HSVShift_float(float4 baseColor, float3 shift, out float4 Out) {
	Out.rgb = HSVShift(baseColor.rgb, shift);
	Out.a = baseColor.a;
}