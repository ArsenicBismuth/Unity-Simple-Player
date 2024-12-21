Shader "Custom/360ToEquirectangular"
{
    Properties
    {
        _MainTex ("360 Texture", 2D) = "white" {}
        _Rotation ("Rotation", Range(0, 360)) = 0
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Rotation;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // Pass through UVs unchanged - they represent lat/long
                o.uv = v.uv;
                return o;
            }

            float2 DirectionToLatLong(float3 dir)
            {
                float3 normalizedCoords = normalize(dir);
                float latitude = acos(normalizedCoords.y);
                float longitude = atan2(normalizedCoords.z, normalizedCoords.x);
                return float2(longitude, latitude) * float2(0.5/UNITY_PI, 1.0/UNITY_PI);
            }

            float3 RotateAroundYInDegrees(float3 vertex, float degrees)
            {
                float alpha = degrees * UNITY_PI / 180.0;
                float sina, cosa;
                sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, -sina, sina, cosa);
                return float3(mul(m, vertex.xz), vertex.y).xzy;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Convert UV (which is now in lat-long space) to 3D direction
                float2 latLong = i.uv;
                float lat = (1.0 - latLong.y) * UNITY_PI;
                float lon = (latLong.x * 2.0 - 1.0) * UNITY_PI;
                
                float3 dir = float3(
                    sin(lat) * cos(lon),
                    cos(lat),
                    sin(lat) * sin(lon)
                );
                
                // Apply rotation if needed
                dir = RotateAroundYInDegrees(dir, _Rotation);
                
                // Convert back to UV coordinates for sampling the 360 texture
                float2 uv = DirectionToLatLong(dir);
                uv = float2(1.0 - uv.x, uv.y); // Flip X to match standard format
                
                // Sample the texture
                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}