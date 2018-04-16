// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VertexColor/Gamereflect" 
{
   Properties 
   {
      BaseMap("BaseMap", 2D) = "white" {}    
      environmentMap("Environment Map", Cube) = "" {}
      reflectivity("reflectivity1", float) =1
      Lighty("Vertex Lighty", float) =1        
   }
   
   SubShader
   {
	  LOD 300

	  Lighting Off
	  ZWrite on
	  
      Pass
      {
		   CGPROGRAM
		   #pragma vertex vert
		   #pragma fragment frag
		   #include "UnityCG.cginc"
		   
           sampler2D BaseMap; 
           samplerCUBE environmentMap;
           
           uniform float reflectivity;
           uniform float Lighty;
           uniform float LightyAni=1; 
           
           struct appdata_t
           {
               float4 pos:POSITION;
               float2 tex:TEXCOORD0;
			   float4 col:COLOR;
               float3 nor:NORMAL;               
           };
           
           struct v2f
           {
               float4 pos:SV_POSITION;
               float2 tex:TEXCOORD0;
               float3 ref:TEXCOORD1;
			   float4 col:TEXCOORD2;	               
           };


           
           v2f vert(appdata_t v)
           {
              v2f o;
              o.pos=UnityObjectToClipPos(v.pos);
              o.tex=v.tex;
              
              float3 I=mul(unity_ObjectToWorld,v.pos).xyz-_WorldSpaceCameraPos;              
              float3 N=normalize(mul((float3x3)unity_ObjectToWorld,v.nor));
              o.ref=reflect(I,N);
              o.col = v.col*LightyAni*Lighty;              
              return o;
           }
           
           float4 frag(v2f i):COLOR
           {
              float4 reflectionColor=texCUBE(environmentMap,i.ref);
              float4 BaseMapColor=tex2D(BaseMap,i.tex.xy*LightyAni);
              
              return lerp(BaseMapColor,reflectionColor,reflectivity)*i.col;
           }
           ENDCG
       }
   }
}