Shader "Custom/FresnelTransparent"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _FresnelPower ("Fresnel Power", Range(1,5)) = 1.0
        _FresnelScale ("Fresnel Scale", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _FresnelPower;
        float _FresnelScale;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = _Color.a;

            // Fresnel Effect
            float fresnelTerm = pow(1.0 - dot(normalize(IN.viewDir), o.Normal), _FresnelPower);
            o.Emission += _FresnelScale * fresnelTerm * _Color.rgb;

            // Apply ambient color
            o.Albedo = c.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb + c.rgb;
            o.Emission += c.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
