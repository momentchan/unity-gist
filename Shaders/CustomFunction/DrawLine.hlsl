#include "Assets/Packages/unity-gist/Cginc/Shape.cginc"

void DrawLine_float(float2 p, float2 a, float2 b, float width, bool soft, out float Out) {
	float o = 0;
	if (soft)
		o = drawLineSoft(p, a, b, 100 * width);
	else
		o = drawLineHard(p, a, b, width);
	Out = o;
}
