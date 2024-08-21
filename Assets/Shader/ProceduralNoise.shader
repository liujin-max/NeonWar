Shader "Custom/PixelGrass"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (0.2, 0.8, 0.2, 1.0) // 基础草地颜色
        _DetailColor("Detail Color", Color) = (0.1, 0.5, 0.1, 1.0) // 草地细节颜色
        _Scale("Noise Scale", Float) = 20.0 // 减小缩放比例使噪点变大
        _Intensity("Detail Intensity", Float) = 0.5 // 细节强度
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _BaseColor;
            fixed4 _DetailColor;
            float _Scale;
            float _Intensity;

            float PerlinNoise(float2 uv)
            {
                uv *= 0.5; // 缩放 UV 坐标来使噪点变大
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 使用 Perlin 噪声生成草地细节
                float noise = PerlinNoise(i.uv * _Scale);
                noise = smoothstep(0.4, 0.6, noise); // 调整平滑函数的阈值
                noise = pow(noise, 3.0); // 增加噪点对比度

                // 混合基础颜色和细节颜色
                fixed4 color = lerp(_BaseColor, _DetailColor, noise * _Intensity);

                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
