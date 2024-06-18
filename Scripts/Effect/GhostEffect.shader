Shader "Unlit/GhostEffect"
{
    Properties
    {
        _FadeFactor("FadeFactor", float) = 0.1
        _Strength("Strength", float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        CGINCLUDE
        #include "UnityCG.cginc"

        sampler2D _Prev;
        sampler2D _Current;

        float4 computeDiff(v2f_img i) : SV_Target{
            float3 prev = tex2D(_Prev, i.uv);
            float3 current = tex2D(_Current, i.uv);
            return saturate(float4(current - prev, 1.0));
        }


        sampler2D _Accu; 
        sampler2D _Diff;

        float _Decay;

        float4 accu(v2f_img i) : SV_Target{
            float3 col = tex2D(_Accu, i.uv) + tex2D(_Diff, i.uv);
            col -= _Decay;
            return saturate(float4(col.rgb, 1.0));
        }

        float _Strength;
        sampler2D _Composite;

        float4 composite(v2f_img i) : SV_Target{
            float3 col = tex2D(_Composite, i.uv);
            float3 accu = tex2D(_Accu, i.uv);
            col += _Strength * accu;
            return saturate(float4(col.rgb, 1.0));
        }

        ENDCG

        Pass
        {
            CGPROGRAM
            #pragma target 5.0
            #pragma vertex vert_img
            #pragma fragment computeDiff
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma target 5.0
            #pragma vertex vert_img
            #pragma fragment accu
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma target 5.0
            #pragma vertex vert_img
            #pragma fragment composite
            ENDCG
        }
    }
}