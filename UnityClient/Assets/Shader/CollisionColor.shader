// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VertexColor/CollisionColor"
{
	Properties
	{
	    _MainColor ("Base (RGB) Color", Color) = (0,0,0,0)
	}

	SubShader
	{
		LOD 1000

        Tags{"IgnoreProjector" = "True" "Queue" = "Geometry" "RenderType" = "Transparent"}


		Pass
		{
		    Cull Off
			Lighting Off
			ZWrite off
		    Fog {Mode Off}
            Blend SrcAlpha OneMinusSrcAlpha
            
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			half4 _MainColor;

			struct appdata_t
			{
				float4 vertex  : POSITION;
				float3 Normal0 : NORMAL;		
			};

			struct v2f
			{
				float4 vertex : POSITION;
				float4 Color0 : TEXCOORD0;				
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex   = UnityObjectToClipPos(v.vertex);
				o.Color0   =_MainColor*clamp(v.Normal0.y,0.5,1.0);
				return o;
			}

			half4 frag (v2f IN) : COLOR
			{
                half4 c;
                c.rgb = IN.Color0.rgb;
                c.a = 0.3f;
                return c;
			}
			ENDCG
		}
	}
	
}
