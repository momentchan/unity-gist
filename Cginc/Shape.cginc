#ifndef SHAPE_INCLUDED
#define SHAPE_INCLUDED

float drawLineSoft(float2 p, float2 a, float2 b, float width) {
	float2 pa = p - a, ba = b - a;
	float h = saturate(dot(pa, ba) / dot(ba, ba));
	float2 d = pa - ba * h;
	float k = dot(d, d);
	return lerp(float4(1, 1, 1, 1), float4(0, 0, 0, 1), step(saturate(width * 1e-3), k));
}

float drawLineHard(float2 p, float2 a, float2 b, float width) {
	float2 p12 = b - a;
	float2 p13 = p - a;

	float d = dot(p12, p13) / length(p12);
	float2 p4 = a + normalize(p12) * d;
	if (length(p4 - p) < width
		&& length(p4 - a) <= length(p12)
		&& length(p4 - b) <= length(p12)) {
		return 1;
	}
	return 0;
}

float drawPolygon(float2 UV, float Sides, float Width, float Height)
{
	float pi = 3.14159265359;
	float aWidth = Width * cos(pi / Sides);
	float aHeight = Height * cos(pi / Sides);
	float2 uv = (UV * 2 - 1) / float2(aWidth, aHeight);
	uv.y *= -1;
	float pCoord = atan2(uv.x, uv.y);
	float r = 2 * pi / Sides;
	float distance = cos(floor(0.5 + pCoord / r) * r - pCoord) * length(uv);
	return saturate((1 - distance) / fwidth(distance));
}

// y = Ax + B
float drawLineEq(float2 UV, float A, float B, float Smoothness) {
	return smoothstep(Smoothness, 0, abs(UV.x * A + B - UV.y));
}
#endif