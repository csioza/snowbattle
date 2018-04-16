Shader "VertexColor/NoBlend"
{
    Properties 
    {
     _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
     _LightPower ("Light Power", Range(0,3)) = 1      
    }
    
    SubShader 
    {
      Tags{"IgnoreProjector" = "True" "Queue" = "Geometry"}
      
      LOD 300
      ZWrite On
      Lighting Off
      Cull Back
      
      CGPROGRAM
      #pragma surface surf BasicDiffuse vertex:vert noforwardadd 
      
      float _LightPower; 
      sampler2D _MainTex;

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
       o.Albedo =tex2D(_MainTex, IN.MTex).rgb*IN.vertColor*_LightPower;
       o.Alpha = 1.0f;
      }     
      
      inline float4 LightingBasicDiffuse (SurfaceOutput s, float3 lightDir, float atten)  
      {  
       float diff = max (0, dot (s.Normal, lightDir));  
         
       float4 c;  
       c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);  
       c.a = 1.0f;  
       return c;  
      }
      ENDCG     
     }
}
