
Shader "TexMix/Tex4Mix" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TexLvl1 ("Lvl1 (RGB)", 2D) = "black" {}
		_TexLvl2 ("Lvl2 (RGB)", 2D) = "black" {}
		_TexLvl3 ("Lvl3 (RGB)", 2D) = "black" {}
		_TexLvl4 ("Lvl4 (RGB)", 2D) = "black" {}
		_TexMix  ("Blend(RGBA)",2D) = "black" {}
	}
	SubShader {
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _TexLvl1;
		sampler2D _TexLvl2;
		sampler2D _TexLvl3;
		sampler2D _TexLvl4;
		
		sampler2D _TexMix;

		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{			
			half4 mc = tex2D (_TexMix,  IN.uv_MainTex);
			half4 c0 = tex2D (_MainTex, IN.uv_MainTex);
			half4 c1 = tex2D (_TexLvl1, IN.uv_MainTex);
			half4 c2 = tex2D (_TexLvl2, IN.uv_MainTex);
			half4 c3 = tex2D (_TexLvl3, IN.uv_MainTex);
			half4 c4 = tex2D (_TexLvl4, IN.uv_MainTex); 
			o.Albedo = lerp(c0, c1, mc.r);
			o.Albedo = lerp(o.Albedo, c2, mc.g);
			o.Albedo = lerp(o.Albedo, c3, mc.b);
			//o.Albedo = lerp(o.Albedo, c4, mc.a);
			//o.Albedo = c0.rgb * mc.r + c1.rgb * mc.g + c2.rgb * mc.b + c3.rgb * mc.a;
			o.Alpha = 1.0;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
