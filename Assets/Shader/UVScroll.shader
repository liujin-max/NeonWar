Shader "Custom/UVScroll"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScrollX ("Scroll X Speed", Range(-10.0, 10.0)) = 0.0
        _ScrollY ("Scroll Y Speed", Range(-10.0, 10.0)) = 0.0
    }
    SubShader
    {
        // Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ScrollX;
            float _ScrollY;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // 手动计算 UV 变换
                o.texcoord = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.texcoord;
                uv.x += _ScrollX * _Time.y;
                uv.y += _ScrollY * _Time.y;
                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
