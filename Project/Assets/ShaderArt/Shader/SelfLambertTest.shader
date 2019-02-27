Shader "Custom/LambetTest" {
	Properties {
		_MainColor("Main Color", Color) = (1, 1, 1, 1)
		_MaterialReflectflg("Material Reflect flg", Range(0, 1)) = 0.5
		_MainTex("MainTex", 2D) = "white"{}
		_NormalTex("_NormalTex", 2D) = "white"{}
		_Gloss("Gloss", Range(0, 1)) = 0.2
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf MyLambert
		#pragma target 3.0
		
		float4 _MainColor;
		float _MaterialReflectflg;
		sampler2D _MainTex;
		sampler2D _NormalTex;
		float _Gloss;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_NormalTex;
		};

		struct LambertOutput
		{
			float3 Albedo;
			float3 Normal;
			float3 Emission;
			half Specular;
			fixed Gloss;
			fixed Alpha;
		};

		void surf(Input IN, inout LambertOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex) * _MainColor * _MaterialReflectflg;
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_NormalTex));
			o.Alpha = c.a;
			o.Gloss = _Gloss;
		}

		fixed4 LightingMyLambert(LambertOutput o, fixed3 lightDir, half3 viewDir, fixed atten)
		{
			fixed4 c;
			fixed3 diffuse = _LightColor0.rgb * _MaterialReflectflg * max(dot(o.Normal, lightDir), 0);
			fixed3 halfVector = normalize(normalize(lightDir) + normalize(viewDir));
			fixed3 specular = _LightColor0.rgb * _MaterialReflectflg * pow(max(dot(o.Normal, halfVector), 0), o.Gloss);
			c.rgb = o.Albedo + diffuse + specular; 
			c.a = o.Alpha;
			return c;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
