Shader "Custom/FlipShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlipX ("Flip X", Int) = 0
    }
    SubShader
    {
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
            float4 _MainTex_ST;
            int _FlipX;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;

                if (_FlipX == 1)
                {
                    o.texcoord.x = 1.0 - o.texcoord.x; // Flip horizontally
                }

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.texcoord);
                return col;
            }
            ENDCG
        }
    }
}
