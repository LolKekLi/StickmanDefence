Shader "Custom/CircleShader" {
    Properties {
        _WorldRadius ("World Radius", Range(0.1, 10.0)) = 1.0
        _Color ("Color", Color) = (1,1,1,1)
        _BorderColor ("Border Color", Color) = (0,0,0,1)
        _BorderWidth ("Border Width", Range(0.0, 0.5)) = 0.1
    }

    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Input properties
            float _WorldRadius;
            float4 _Color;
            float4 _BorderColor;
            float _BorderWidth;

            // Vertex shader
            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float worldRadius : TEXCOORD0;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldRadius = _WorldRadius / length(UnityObjectToClipPos(v.vertex).xyz);
                return o;
            }

            // Fragment shader
            float4 frag (v2f i) : SV_Target {
                float2 pos = i.vertex.xy / i.vertex.w;
                float dist = length(pos);
                float alpha = smoothstep(i.worldRadius, i.worldRadius - _BorderWidth, dist) - smoothstep(i.worldRadius, i.worldRadius + _BorderWidth, dist);
                float4 col = lerp(_BorderColor, _Color, alpha);
                return col;
            }
            ENDCG
        }
    }
}
