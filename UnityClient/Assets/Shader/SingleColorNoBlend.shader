// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VertexColor/SingleColor"
{
	Properties
	{
	    _MainColor ("Base (RGB) Color", Color) = (0,0,0,0)
	}

	SubShader
	{
		LOD 1000

		Tags
		{
			"Queue" = "Geometry-200"
			"IgnoreProjector" = "True"
			"RenderType" = "Geometry"
		}

		Pass
		{
			Lighting Off
			ZWrite off
		    Fog {Mode Off} 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			half4 _MainColor;

			struct appdata_t
			{
				float4 vertex : POSITION;		
			};

			struct v2f
			{
				float4 vertex : POSITION;				
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			half4 frag (v2f IN) : COLOR
			{
				return _MainColor;
			}
			ENDCG
		}
	}
	
}
