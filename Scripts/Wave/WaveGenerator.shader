Shader"Unlit/WaveGenerator"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        CGINCLUDE
        #include "UnityCG.cginc"

        float _Width;
        float _FadeFactor;
        float _Distortion;
        float _T;

        int _WaveCount;

        struct WaveData
        {
            bool active;
            float ratio;
        };

        StructuredBuffer<WaveData> _Waves;

        float remap(float In, float2 InMinMax, float2 OutMinMax)
        {
            return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }

        float4 ComputeMaskValue(float2 uv, float x, int id)
        {
            uv.x += sin(uv.y * 2.5 + _T  * 0.1 + id) * _Distortion;
            
            float4 col = 0;
            float len = abs(x - uv.x);
            col.rgb += smoothstep(_Width, 0, len);
            return col;
        }

        float4 frag(v2f_img i) : SV_Target
        {
            float4 col = 0;
            for (int id = 0; id < _WaveCount; id++)
            {
                WaveData d = _Waves[id];
                if (d.active)
                {
                    col += ComputeMaskValue(i.uv, remap(d.ratio, float2(0, 1), float2(-0.2, 1.2)), id) * 0.5;
                }
            }
    
            col -= _FadeFactor*0.9;
            return saturate(col);
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