Shader "Self-Illumin/Vertex_Bump" {
    Properties {
    	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
    	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BumpMap ("Bumpmap", 2D) = "bump" {}
		_IllumColor ("Illum Color", Color) = (1, 1, 1, 1)
		_Illum ("Illumin (A)", 2D) = "white" {}
		
		_IllumStartDistance ("Illum Start Distance", Range (0, 80)) = 10
		_IllumSoftenDistance ("Illum Soften Distance", Range (0, 25)) = 10
		
		_EmissionLM ("Emission (Lightmapper)", Float) = 0
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque"}       
		
	    CGPROGRAM
        #pragma exclude_renderers gles

	    #include "UnityCG.cginc"
		#pragma surface surf BlinnPhong vertex:vert

			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _Illum;
			fixed4 _IllumColor;
			half _Shininess;
			fixed _IllumStartDistance;
			fixed _IllumSoftenDistance;
			
			struct Input {
			  float2 uv_MainTex;
			  float2 uv_BumpMap;
			  float2 uv_Illum;
			  float3 customColor;
			  float fooAlpha;
			};
			
			void vert (inout appdata_full v, out Input o) {
			  	UNITY_INITIALIZE_OUTPUT(Input,o);
			  	o.customColor = v.color;
			  
			  	float3 foo = mul(UNITY_MATRIX_MVP, v.vertex);
			  	float zeroToOne = (foo.z - _IllumStartDistance) / _IllumSoftenDistance;
			  	zeroToOne = abs(clamp(zeroToOne, 0.0, 1.0)-1);
   				o.fooAlpha = zeroToOne;
			}
			
			void surf (Input IN, inout SurfaceOutput o) {
				fixed4 tex = tex2D (_MainTex, IN.uv_MainTex);
				fixed4 illumText = tex2D(_Illum, IN.uv_MainTex);

			  	o.Albedo = tex.rgb * IN.customColor;
			  	o.Gloss = tex.a;
			  	o.Emission = _IllumColor.rgb * illumText.a * illumText.rgb * IN.fooAlpha;
			  	o.Specular = _Shininess;
			  	o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_MainTex));
			}
	    ENDCG
        
    }
    FallBack "Diffuse"
}