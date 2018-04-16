Shader "VertexColor/MutilLayer"
{
  Properties 
  {
	_MainTex ("Base (RGB), Alpha (A)", 2D) =   "" {}

    _Layer0  ("_Layer0", 2D)   = "white" {}
	_Layer1  ("_Layer`", 2D)   = "white" {}
	_Layer2  ("_Layer1", 2D)   = "white" {}

    _TileRepeat0 ("Tiling Repeat0", Range(0,50)) = 10   
    _TileRepeat1 ("Tiling Repeat1", Range(0,50)) = 10    
	_TileRepeat2 ("Tiling Repeat2", Range(0,50)) = 10
  }
	
  SubShader 
  {
  
	Tags{"SplatCount" = "3" "IgnoreProjector" = "True" "Queue" = "Geometry-200"}
	
	LOD 300
	ZWrite on
    Lighting Off    
    Cull Back
    
    CGPROGRAM
    #pragma surface surf BasicDiffuse vertex:vert noforwardadd 
   
    sampler2D _MainTex;
    sampler2D _Layer0,_Layer1,_Layer2;
    half _TileRepeat0,_TileRepeat1,_TileRepeat2;
    
    uniform float Uani;
    uniform float Vani;
    
    struct Input 
    {   
      float2 MTex0;
      float2 MTex1;      
    };
    
    void vert(inout appdata_full v, out Input o)    
    {   
      o.MTex0=float2(v.texcoord.x,v.texcoord.y);
      o.MTex1=float2(v.texcoord.x+Uani,v.texcoord.y+Vani);
    }
    
    void surf (Input IN, inout SurfaceOutput o) 
    {
     half4 col;
     half4 splat_control= tex2D(_MainTex, IN.MTex0);
     
     col.rgb   = ((1.0f-splat_control.g)*splat_control.r)* tex2D (_Layer0, IN.MTex0*_TileRepeat0).rgb;
     col.rgb  += ((1.0f-splat_control.g)*(1.0f-splat_control.r))* tex2D (_Layer1, IN.MTex0*_TileRepeat1).rgb;
     col.rgb  +=  splat_control.g * tex2D (_Layer2, IN.MTex1*_TileRepeat2).rgb;
     col.rgb  *=  splat_control.b;
          
     o.Albedo = col.rgb;
     o.Alpha = 1.0f;
    }
    
    inline float4 LightingBasicDiffuse (SurfaceOutput s, float3 lightDir, float atten)  
    {  
     float diff = max (0, dot (s.Normal, lightDir));  
       
     float4 c;  
     c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten*2 );  
     c.a = 1.0f;  
     return c;  
    }
    ENDCG  
  }
}
