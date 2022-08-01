#ifndef __FBM_INCLUDE__
#define __FBM_INCLUDE__

#include "SimplexNoise3D.cginc"

float fbm4(float2 p, float t)
{
	float f;
	f = 0.50000 * snoise(float3(p, t)); p = p * 2.01;
	f += 0.25000 * snoise(float3(p, t)); p = p * 2.02; //from iq
	f += 0.12500 * snoise(float3(p, t)); p = p * 2.03;
	f += 0.06250 * snoise(float3(p, t));
	return f;
}

float fbm3(float2 p, float t)
{
	float f;
	f = 0.50000 * snoise(float3(p, t)); p = p * 2.01;
	f += 0.25000 * snoise(float3(p, t)); p = p * 2.02; //from iq
	f += 0.12500 * snoise(float3(p, t));
	return f;
}

float fbm2(float2 p, float t)
{
	float f;
	f = 0.50000 * snoise(float3(p, t)); p = p * 2.01;
	f += 0.25000 * snoise(float3(p, t));
	return f;
}

#endif