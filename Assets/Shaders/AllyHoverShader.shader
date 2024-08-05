Shader "Custom/AllyHoverShader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Range(0, 0.03)) = 0.03
    }

    SubShader
    {
        Tags {"Queue"="Overlay" }
        Lighting Off
        ZWrite Off
        Cull Off
        Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 because it uses wrong array syntax (type[size] name)
#pragma exclude_renderers d3d11
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
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _OutlineColor;
            float _OutlineThickness;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = _OutlineColor;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 mainTexCoord = i.texcoord;
                half4 mainTexColor = tex2D(_MainTex, mainTexCoord);

                float2 outlineOffsets[8] = float2[8](
                    float2(-1, -1), float2(-1, 1), float2(1, 1), float2(1, -1),
                    float2(-1, 0), float2(1, 0), float2(0, -1), float2(0, 1)
                );

                half outlineAlpha = 0.0;

                for (int j = 0; j < 8; j++)
                {
                    float2 offsetTexCoord = mainTexCoord + outlineOffsets[j] * _OutlineThickness;
                    half4 offsetTexColor = tex2D(_MainTex, offsetTexCoord);
                    outlineAlpha = max(outlineAlpha, offsetTexColor.a);
                }

                half4 outlineColor = half4(i.color.rgb, outlineAlpha * i.color.a);
                return lerp(mainTexColor, outlineColor, outlineColor.a);
            }
            ENDCG
        }
    }
}
