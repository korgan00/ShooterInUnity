// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.
Shader "HogansAlley/FuturisticUnlit" {
    Properties{
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _LinesHeight("Lines Height", Range(0,1)) = 0.5
        _LinesAlpha("Alpha", Range(0,1)) = 0.5
    }

    SubShader{
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass{
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
                #pragma multi_compile_fog

                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                    float2 texcoord : TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                        UNITY_VERTEX_OUTPUT_STEREO
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                fixed _LinesHeight;
                fixed _LinesAlpha;

                v2f vert(appdata_t v) {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    fixed f = (sin((i.texcoord.y + _SinTime.x / 4) * 1000 * _LinesHeight) * 0.5) + 0.5f;
                    fixed4 col = tex2D(_MainTex, i.texcoord);

                    col.a = 1 - pow(f, 10 * _LinesAlpha);

                    UNITY_APPLY_FOG(i.fogCoord, col);
                    return col;
                }
            ENDCG
        }
    }
}

/*
Shader "Gregorio/Diffuse" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _LinesHeight("Lines Height", Range(0,1)) = 0.5
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Lambert
        #pragma target 2.0

        sampler2D _MainTex;
        fixed _Cutoff;
        fixed _LinesHeight;

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            
            fixed f = (sin((IN.uv_MainTex.y + _SinTime.x/4) * 1000 * _LinesHeight) / 2) + 0.5f;

            o.Albedo = c.rgb * f;
            o.Alpha = c.a * 0.5;
        }
        ENDCG
    }

    Fallback "Mobile/VertexLit"
}

*/