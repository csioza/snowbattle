Shader "VertexColor/LpCullOff"
{	
	Properties 
	{
	 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	 _LightMap ("Base (RGB), Alpha (A)", 2D) = "white" {}		 
	 _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	
	SubShader
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 300
		
		BindChannels 
		{
			Bind "texcoord", texcoord0 // main uses 1st uv
			Bind "texcoord1", texcoord1 // lightmap uses 2nd uv			
		}
				
	   Pass 
	   {
		 Cull Off
		 Lighting Off
		 ZWrite On	 
		 Alphatest Greater [_Cutoff]

		 SetTexture [_MainTex] 
		 {
			combine texture 
		 }
		 SetTexture [_LightMap] 
		 {
			combine texture * previous DOUBLE
		 }

 		 
	   }
	}

}
