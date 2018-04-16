// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VertexColor/LPNoBlend"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
		_LpTex ("Base (RGB), Alpha (A)", 2D) = "white" {}		
	}

	SubShader
	{
		LOD 300

		Tags
		{
			"Queue" = "Geometry-100"
			"IgnoreProjector" = "True"
			"RenderType" = "Opaque"
		}
		
		Lighting Off
		ZWrite on
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _LpTex;
			
            uniform float LightyAni=1;     
            uniform float Hani=0;
            uniform float Vani=0;
                   
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoordLp : TEXCOORD1;		
			};

			struct v2f
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoordLp : TEXCOORD1;				
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
			    o.texcoord.x = v.texcoord.x+Hani;
			    o.texcoord.y = v.texcoord.y+Vani;	
				o.texcoordLp = v.texcoordLp;
				return o;
			}

			half4 frag (v2f IN) : COLOR
			{
				float4 col = tex2D(_MainTex, IN.texcoord) * (tex2D(_LpTex, IN.texcoordLp)*LightyAni);		
				return col;
			}
			ENDCG
		}
	}
	
}
