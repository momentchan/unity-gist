Shader "Unlit/StencilBuffer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (0,0,0,1)
        _Stencil("Stencil", int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Stencil
        {
            Ref[_Stencil]
            Comp Always
            Pass Replace
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Color;

            fixed4 frag (v2f_img i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}
