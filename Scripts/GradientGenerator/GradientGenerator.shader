Shader "Hidden/GradientGenerator"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"
            sampler2D _MainTex;
            uniform float4 _MainTex_TexelSize;

            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 col = 0;

                fixed i_00 = tex2D(_MainTex, i.uv + float2(0, 0) * _MainTex_TexelSize);
                fixed i_10 = tex2D(_MainTex, i.uv + float2(-1, 0) * _MainTex_TexelSize);
                fixed i10 = tex2D(_MainTex, i.uv + float2(1, 0) * _MainTex_TexelSize);
                fixed i01 = tex2D(_MainTex, i.uv + float2(0, 1) * _MainTex_TexelSize);
                fixed i0_1 = tex2D(_MainTex, i.uv + float2(0, -1) * _MainTex_TexelSize);

                col.rg = float2(i10 - i_10, i01 - i0_1) * 10;
                return col;
            }
            ENDCG
        }
    }
}
