Shader "VertexLit/Diffuse_Bump_Detail" {
    Properties {
    	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
    	_Shininess ("Shininess", Range (0.03, 4)) = 0.078125
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BumpMap ("Bumpmap", 2D) = "bump" {}
		_ParallaxMap ("Heightmap (A)", 2D) = "black" {}
		_Parallax ("Height", Range (0.005, 0.08)) = 0.02

		_DetailBumpMap ("Detail Bumpmap", 2D) = "bump" {}
    }
    SubShader 
    {
        Tags {"RenderType"="Opaque"}
		
        CGPROGRAM
            #include "UnityCG.cginc"
            #pragma surface surf BlinnPhong vertex:vert
            #pragma target 3.0
			
		    sampler2D _MainTex;
		    sampler2D _BumpMap;
		    sampler2D _DetailBumpMap;
		    sampler2D _ParallaxMap;
		    half _Shininess;
		    float _Parallax;
			
		    struct Input {
			    float2 uv_MainTex;
			    float2 uv_BumpMap;
			    float2 uv_DetailBumpMap;
			    float3 customColor;
			    float3 viewDir;
		    };
			
		    void vert (inout appdata_full v, out Input o) {
			    UNITY_INITIALIZE_OUTPUT(Input, o);
			    o.customColor = v.color;
		    }
			
		    void surf (Input IN, inout SurfaceOutput o) {
			    half h = tex2D (_ParallaxMap, IN.uv_BumpMap).w;
			    float2 offset = ParallaxOffset (h, _Parallax, IN.viewDir);
				
			    IN.uv_BumpMap += offset;
			    IN.uv_DetailBumpMap += offset;
			
			    fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
				
			    o.Albedo = tex.rgb * IN.customColor.rgb;
			    o.Gloss = tex.a;
			    o.Specular = _Shininess;
			    o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)) + UnpackNormal(tex2D (_DetailBumpMap, IN.uv_DetailBumpMap));
		    }
	    ENDCG
        
    }
    FallBack "Bumped Specular"
}