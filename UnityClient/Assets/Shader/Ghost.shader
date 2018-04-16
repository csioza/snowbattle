// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Ghost/Blend" {
	Properties {
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_NoiseShade ("Noise Texture(RGB)", 2D) = "white" {}
		
		_GhostColor ("Ghost Color", Color) = (1,0,0,1)
		_FresnelWeight("Ghost Weight",Range (.00, 0.4)) = 0.02
		_FresnelPower("Ghost Power",Range (.001, 32)) = 2
		_FresnelScale("Ghost Scale",Range (0, 1)) = 0
	}


	SubShader {
		Tags {"Queue"="AlphaTest+9" "RenderType"="Opaque" }
		Pass {
			Name "BASE"
			//Cull Off
			ZWrite Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _NoiseShade;
			float4 _MainTex_ST;
			float4 _Color;
			float4 _GhostColor;
			float _FresnelWeight;
			float _FresnelPower;
			float _FresnelScale;

			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};
			
			struct v2f {
				float4 pos : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 ScreenTC:TEXCOORD1;
				float3 fresnel : TEXCOORD2;
				//float3 cubenormal:TEXCOORD2;
			};
			
			float schlick_fresnel(float3 incident, float3 normal,float fresnelPower, float r0)
			{
				// R0 = ((eta - 1) / (eta + 1)) ^ 2;
				float e_n = max(dot(incident, normal), 0);
				return saturate(r0 + (1 - r0) * pow(1 - e_n, fresnelPower));
			}
			float4 HPosToScreenTC(float4 HPos)
			{
				float4 ScrTC = HPos;
				ScrTC.xy = (HPos.xy * float2(1,-1) + HPos.ww  ) * 0.5;
				return ScrTC;
			}
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				float3 eyepos = _WorldSpaceCameraPos;
				//o.cubenormal = mul (UNITY_MATRIX_MV, float4(v.normal,0));
				float3 localEyePos = mul((float3x3)unity_WorldToObject ,eyepos);
				float3 pos_to_eye=normalize(localEyePos.xyz-v.vertex.xyz);
				//o.fresnel.g=1
				//o.fresnel.b=1
				o.ScreenTC = HPosToScreenTC(o.pos);
				o.fresnel = _GhostColor * schlick_fresnel(pos_to_eye,v.normal,_FresnelPower,_FresnelWeight);
				//o.pos.zw=o.pos.zw*1.1;
				//o.pos.w=o.pos.w*0.999;
				//o.pos.z=o.pos.z+0.001;
				//o.fresnel.xy = o.fresnel.xy*(_ScreenParams.xy)/16;
				//o.fresnel.xy = o.fresnel.xy*(_ScreenParams.xy/16);
				//_FresnelColor * schlick_fresnel(pos_to_eye,v.normal,_FresnelPower,_FresnelWeight);
				return o;
			}

			float4 frag (v2f i) : COLOR
			{
				float4 col = _Color * tex2D(_MainTex, i.texcoord);
				//float4 cube = texCUBE(_ToonShade, i.cubenormal);
				float2 uv = i.ScreenTC.xy/i.ScreenTC.w;
				uv = uv*_ScreenParams.xy /16;
				//uv.y = uv.y*2;
				//uv.y = uv.y*3;
				float4 noise = tex2D(_NoiseShade,uv);
				clip(noise.r-0.5);
				//return noise;
				//return float4(0,0,i.fresnel.y*0.1,1);
				//return col;
				//return float4(i.fresnel.x,0,0,1);
				//return float4(i.fresnel + 2.0*col.rgb*cube.rgb, col.a);
				return float4(_GhostColor.xyz*_FresnelScale+col.rgb+i.fresnel,col.a);
			}
			ENDCG			
		}
	} 

	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
			Name "BASE"
			Cull Off
			SetTexture [_MainTex] {
				constantColor [_Color]
				Combine texture * constant
			} 
			SetTexture [_ToonShade] {
				combine texture * previous DOUBLE, previous
			}
		}
	} 
	
	Fallback "VertexLit"
}
