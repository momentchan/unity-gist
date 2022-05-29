Shader "Unlit/MaskGenerator"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        CGINCLUDE
        #include "UnityCG.cginc"

        sampler2D _MaskTex;

        float _Radius;
        float _Smoothness;
        float _FadeFactor;

        float _Aspect;
        float _Strength;
        float2 _Position;
        float2 _Direction;

        float4 ComputeMaskValue(float2 uv) {
            float4 col = 0;
            float2 dis = (_Position - uv) * float2(_Aspect, 1) * _Direction;
            float len = length(dis);
            col.rgb += smoothstep(_Radius, _Radius * _Smoothness, len) * 0.5;
            return col;
        }

        float4 single_frame(v2f_img i) : SV_Target {
            float4 col = 0;
            col += ComputeMaskValue(i.uv);
            return col;
        }

        float4 accumulation(v2f_img i) : SV_Target{
            float4 col = tex2D(_MaskTex, i.uv);
            col += _Strength * ComputeMaskValue(i.uv);
            col -= _FadeFactor;
            return saturate(col);
        }

        ENDCG

        Pass
        {
            CGPROGRAM
            #pragma target 5.0
            #pragma vertex vert_img
            #pragma fragment single_frame
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma target 5.0
            #pragma vertex vert_img
            #pragma fragment accumulation
            ENDCG
        }
    }
}