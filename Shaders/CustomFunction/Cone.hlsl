#include "Circle.hlsl"
#include "Assets/Packages/unity-gist/Cginc/Noise.cginc"


float DrawCircle(float2 UV, float2 Center, float Radius){
	float d = distance(UV, Center);
	return 1 - smoothstep(0, Radius, d);
}

void Cone_float(float2 UV, float2 Center, float2 Shift, float2 Radius, float DecayPower, float Iteration, float Frequency, float Seed, float Ratio, out float Out) {
	
	Out = 0;

	for(int i=0; i < Iteration; i++){
		float2 Shift_orth = normalize(float2(Shift.y, -Shift.x));

		float rn = snoise(float2(i * Frequency, Seed))* 0.001;

		float r = i/Iteration;
		float strength = pow(1 - r, DecayPower);
		strength *= 1 - smoothstep(0, 1, abs(r-lerp(-1, 1.5, Ratio)));

		Out += saturate(DrawCircle(UV, Center + (Shift  + rn * Shift_orth) * i , lerp(Radius.x, Radius.y, r)) * strength);// * //pow(1-r, DecayPower);
		Out = clamp(Out, 0, 1);
	}
}



