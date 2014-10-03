Shader "Custom/Vertex_DiffuseBumpTransparent" {
    Properties {
    	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
    	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BumpMap ("Bumpmap", 2D) = "bump" {}
		_ParallaxMap ("Heightmap (A)", 2D) = "black" {}
		_Parallax ("Height", Range (0.005, 0.08)) = 0.02
    }
    SubShader 
    {
        Tags {"RenderType"="Transparent"}
		
	    CGPROGRAM
			#pragma surface surf BlinnPhong vertex:vert alpha
			#pragma target 3.0
			
			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _ParallaxMap;
			half _Shininess;
			float _Parallax;
			
			struct Input {
			  float2 uv_MainTex;
			  float2 uv_BumpMap;
			  float4 customColor;
			  float3 viewDir;
			};
			
			void vert (inout appdata_full v, out Input o) {
			  UNITY_INITIALIZE_OUTPUT(Input,o);
			  o.customColor = v.color;
			}
			
			void surf (Input IN, inout SurfaceOutput o) {
				half h = tex2D (_ParallaxMap, IN.uv_MainTex).w;
				float2 offset = ParallaxOffset (h, _Parallax, IN.viewDir);
				IN.uv_MainTex += offset;
			
				fixed4 tex = tex2D (_MainTex, IN.uv_MainTex);
			  	o.Albedo = tex.rgb * IN.customColor.rgb; //Textur + VertexColors
			  	o.Gloss = tex.a;
			  	o.Specular = _Shininess;
			  	o.Alpha = IN.customColor.a;
			  	o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_MainTex));
			}
	    ENDCG
        
    }
    FallBack "Diffuse"
}