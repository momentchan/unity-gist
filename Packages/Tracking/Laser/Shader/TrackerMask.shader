Shader "Unlit/TrackerMask"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        CGINCLUDE
        #include "UnityCG.cginc"
        #include "TrackerData.cginc"
        
        StructuredBuffer<TrackerData> _TrackerBuffer;
        int _TrackerNum;

        float _Radius;
        float _Aspect;

        float4 ComputeMaskValue(float2 uv, TrackerData d)
        {
            float4 col = 0;
            float2 dis = (d.pos - uv) * float2(_Aspect, 1);
            float len = length(dis);
            col.rgb += smoothstep(_Radius, _Radius * 0.1, len) * d.activeRatio;
            return col;
        }

        float4 frag(v2f_img i) : SV_Target
        {
            float4 col = 0;
            for (int t = 0; t < _TrackerNum; t++)
            {
                TrackerData d = _TrackerBuffer[t];
                col += ComputeMaskValue(i.uv, d);
            }
            return col;
        }
        ENDCG

        Pass
        {
            CGPROGRAM
            #pragma target 5.0
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }
}