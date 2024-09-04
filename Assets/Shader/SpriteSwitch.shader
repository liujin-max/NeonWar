Shader "Custom/SpriteSwitch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tex ("OTexture", 2D) = "white" {}
        _Amount ("Amount", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            sampler2D _Tex;
            float _Amount;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                fixed4 iceCol = tex2D(_Tex, i.texcoord);

                // 将冰冻效果混合到图片中
                col = lerp(col, iceCol, _Amount);

                return col;
            }
            ENDCG
        }
    }
}
