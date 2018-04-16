// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VertexColor/Blend"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
        Lighty("Vertex Lighty", float) =1
	}

	SubShader
	{
		LOD 300

		Tags
		{
			"Queue" = "Transparent+100"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
	    Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
            uniform float Lighty;
            uniform float LightyAni=1;             
            uniform float Hani=0;
            uniform float Vani=0;    
                                    
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color : COLOR;				
			};

			struct v2f
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color : TEXCOORD1;				
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
			    o.texcoord.x = v.texcoord.x+Hani;
			    o.texcoord.y = v.texcoord.y+Vani;			    
				o.color = v.color*LightyAni*Lighty;
				return o;
			}

			half4 frag (v2f IN) : COLOR
			{
				float4 col = tex2D(_MainTex, IN.texcoord);
				col.rgb*=IN.color.rgb;
				
				return col;
			}
			ENDCG
		}
	}
	
}
