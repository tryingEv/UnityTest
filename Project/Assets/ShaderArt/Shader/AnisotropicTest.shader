Shader "Custom/AnisotropicTest" {
	Properties{
		_MainTint ("Diffuse Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
		_Specular ("Specular Amount", Range(0, 1)) = 0.5
		_SpecularPow ("Specular Power", Range(0, 1)) = 0.5
		_AnisoDir ("Anisotropic Direction", 2D) = "" {}
		_AnisoOffset ("Anisotropic Offset", Range(-1, 1)) = -0.2
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Ansiotropic

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _AnisoDir;
			float4 _MainTint;
			float4 _SpecularColor;
			float _SpecularPow;
			float _AnisoOffset;
			float _Specular;

			struct Input
			{
				float2 uv_MainTex;
				float2 uv_AnisoDir;
			};

			struct SurfaceAnisoOutput
			{
				fixed3 Albedo;
				fixed3 Normal;
				fixed3 Emission;
				fixed3 AnisoDirection;
				half Specular;
				fixed Gloss;
				fixed Alpha;
			};

			void surf(Input IN, inout SurfaceAnisoOutput o)
			{
				half4 c = tex2D(_MainTex, IN.uv_MainTex) * _MainTint;
				float3 anisoTex = UnpackNormal(tex2D(_AnisoDir, IN.uv_AnisoDir));
				o.AnisoDirection = anisoTex;
				o.Specular = _Specular;
				o.Gloss = _SpecularPow;
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}

			fixed4 LightingAnsiotropic(SurfaceAnisoOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
			{
				fixed3 halfVector = normalize(normalize(lightDir) + normalize(viewDir));
				float dotL = saturate(dot(s.Normal, lightDir));
				fixed hdotA = dot(normalize(s.Normal + s.AnisoDirection), halfVector);
				float aniso = max(0, sin(radians((hdotA + _AnisoOffset) * 180)));
				float spec = saturate(pow(aniso, s.Gloss * 128) * s.Specular);
				fixed4 c;
				c.rgb = ((s.Albedo * _LightColor0.rgb * dotL) + (_LightColor0.rgb * _SpecularColor.rgb * spec)) * atten;
				c.a = s.Alpha;
				return c;
			}

			
			ENDCG
		}
			FallBack "Diffuse"
}
