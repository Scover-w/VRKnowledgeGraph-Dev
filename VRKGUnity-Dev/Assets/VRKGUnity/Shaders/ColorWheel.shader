Shader "Custom/RadialGradient"
{
    Properties
    {
        _V ("V Value", Range(0,1)) = 0.5
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

            float _V;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            //float3 HSVToRGB( float3 c )
			//{
			//	float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			//	float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			//	return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			//}

            float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				//return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
			}


            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv * 2 - 1;
                float angle = atan2(uv.y, uv.x) / (2 * UNITY_PI) + 0.5;
                float radius = length(uv);

                if (radius <= 1.0)
                {
                    fixed4 color = fixed4(HSVToRGB(float3(angle, radius, _V)), 1);
                    return color;
                }
                else
                {
                    discard;
                    return fixed4(0,0,0,0);
                }
            }
            ENDCG
        }
    }
}

