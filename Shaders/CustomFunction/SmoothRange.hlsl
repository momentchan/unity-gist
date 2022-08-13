void SmoothRange_float(float2 UV, float2 from, float2 to, float2 smoothness, out float2 Out) {
	Out = smoothstep(from, from + smoothness, UV) * (1 - smoothstep(to - smoothness, to, UV));
}