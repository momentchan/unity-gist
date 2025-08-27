void Circle_float(float2 UV, float2 Center, float OuterSize, float InnerSize, out float Out) {
	float smoothness = (OuterSize - InnerSize) * 0.1;
	float d = distance(UV, Center);
	Out = (1- smoothstep(OuterSize - smoothness, OuterSize, d)) * smoothstep(InnerSize, InnerSize + smoothness, d);
}
