Shader "VertexColor/FullSelfBlend"
{
    Properties 
    {
     _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}   
     _LightColor ("Self Light Color", Color) = (1,1,1,1)      
    }
    
    SubShader 
    {
      Tags{"IgnoreProjector" = "True" "Queue" = "Transparent" "RenderType" = "Transparent"}

      Cull Off
      Lighting Off
      ZWrite off
      Blend SrcAlpha OneMinusSrcAlpha
      CGPROGRAM
      #pragma surface surf BasicDiffuse vertex:vert noforwardadd 

      sampler2D _MainTex;
      sampler2D _LightMap;
      float4 _LightColor;
      
      uniform float Uani;
      uniform float Vani;     
            
      struct Input 
      {
        float2 MTex;
        float4 vertColor;      
      };
      
      void vert(inout appdata_full v, out Input o)    
      {    
            o.vertColor = v.color;    
            o.MTex=float2(v.texcoord.x+Uani,v.texcoord.y+Vani);            
      } 
      
      void surf (Input IN, inout SurfaceOutput o) 
      {     
       float4 Col=tex2D(_MainTex, IN.MTex);
       o.Albedo =Col.rgb*IN.vertColor;
       o.Alpha = Col.a;
      }
       
      inline float4 LightingBasicDiffuse (SurfaceOutput s, float3 lightDir, float atten)  
      {  
       float4 c;  
       c.rgb = s.Albedo;  
       c.a = s.Alpha;
       return c;  
      }
      ENDCG     
     }
}
