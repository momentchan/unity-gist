Shader "Hidden/StencilMask"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Stencil("Stencil", int) = 1
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			Stencil
			{
				Ref[_Stencil]
				Comp NotEqual
			}

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;

			fixed4 frag(v2f_img i) : SV_Target
			{
				return 0;
			}
			ENDCG
		}

		Pass
		{
			Stencil
			{
				Ref[_Stencil]
				Comp Equal
			}

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;

			fixed4 frag(v2f_img i) : SV_Target
			{
				return 1;
			}
			ENDCG
		}
	}
}
