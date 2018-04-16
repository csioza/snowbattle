// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Toon/Basic Outline" {
//#pragma multi_compile FANCY_STUFF_OFF FANCY_STUFF_ON
	Properties {
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (.001, 0.01)) = .005
		_MainTex ("Base (RGB)", 2D) = "white" { }
		_ToonShade ("ToonShader Cubemap(RGB)", CUBE) = "" { Texgen CubeNormal }
		
		
		_FresnelColor ("Fresnel Color", Color) = (1,0,0,1)
		_FresnelWeight("Fresnel Weight",Range (.001, 0.4)) = 0.02
		_FresnelPower("Fresnel Power",Range (.001, 32)) = 2
		_FresnelScale("Fresnel Scale",Range (0, 1)) = 0
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	
	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f {
		float4 pos : POSITION;
		float4 color : COLOR;
	};
	
	uniform float _Outline;
	uniform float4 _OutlineColor;
	
	v2f vert(appdata v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);

		float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
		float2 offset = TransformViewToProjection(norm.xy);

		//o.pos.xy += offset * o.pos.z * _Outline;
		//o.color = _OutlineColor;
		o.color.rgb = float3(0.1,0.3,0.7);//_OutlineColor;
		o.color.a = offset.x*offset.y+0.5;
		return o;
	}
	ENDCG

	SubShader {
		Tags {"Queue"="AlphaTest+10" "RenderType"="Opaque" }
		Pass {
			//Tags {"Queue"="AlphaTest+10" "RenderType"="Opaque" }
			//Tags {"Queue"="Transparent+10" "RenderType"="Opaque" }
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }
			//Cull Back
			ZWrite Off
			//ColorMask RGB
			Blend SrcAlpha One
			//Blend One Zero
			ZTest Greater

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			half4 frag(v2f i) :COLOR { return i.color; }
			ENDCG
		}
		UsePass "Toon/Basic/BASE"
	}
	
	
	Fallback "Toon/Basic"
}
