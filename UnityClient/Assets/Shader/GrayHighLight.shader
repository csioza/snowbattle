﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "XProject/GrayLight" 
{
	Properties 
	{
		_MainTex ("Particle Texture", 2D) = "white" {}
		_TintColor("Tint Color",Color) = (0.5,0.5,0.5,0.5)
	}
	Category 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		Lighting Off 
		
		SubShader 
		{
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _MainTex_ST;
				half4 _TintColor;
			
				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};
			
				struct v2f
				{
					float4 pos : SV_POSITION;
					fixed4 color : COLOR;
					half2 uv : TEXCOORD0;
				};
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
					o.color = v.color;
					return o;
				}
			   

				half4 frag(v2f  i ) : COLOR
				{	
				    half4 tc = tex2D(_MainTex, i.uv);
				    half gray = tc.r*0.3+tc.g*0.6+tc.b*0.1;
					return 2.0f * i.color * _TintColor * tc+gray*_TintColor.a*4.0f;
				}	
				ENDCG           
			}
		} 
		FallBack "Diffuse"
	}
}