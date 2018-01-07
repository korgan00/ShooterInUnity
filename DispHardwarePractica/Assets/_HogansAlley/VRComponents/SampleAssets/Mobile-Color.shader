// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified Bumped shader. Differences from regular Bumped one:
// - no Main Color
// - Normalmap uses Tiling/Offset of the Base texture
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Mobile/Color" {
    Properties {
        _Color("Color", Color) = (1, 1, 1, 1)
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 250

        CGPROGRAM
            #pragma surface surf Lambert noforwardadd

            fixed4 _Color;

            struct Input {
                float2 uv_MainTex;
            };

            void surf (Input IN, inout SurfaceOutput o) {
                o.Albedo = _Color.rgb;
                o.Alpha = _Color.a;
            }
        ENDCG
    }

    FallBack "Mobile/Diffuse"
}
