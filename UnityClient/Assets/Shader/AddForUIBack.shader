// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "XProject/AddForUIBack" 
{
	Properties 
	{
		_MainTex ("Particle Texture", 2D) = "white" {}
		_TintColor("Tint Color",Color) = (0.5,0.5,0.5,0.5)
	}
	
	Category 
	{
		Tags { "Queue"="Transparent-100" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha One
		Cull Off 
		Lighting Off 
		ZWrite Off 
		Fog { Color (0,0,0,0) }
		ColorMask RGB
		
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
					return 2.0f * i.color * _TintColor * tex2D(_MainTex, i.uv);
				}	
				ENDCG           
			}
		} 
		FallBack "Diffuse"
	}
}
