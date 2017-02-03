Shader "Custom/NewSurfaceShader" {
	Properties {
		_Level1 ("Layer 1", float) = .2
		_Color1 ("Color 1", Color) = (0,0,0,1)
		_Level2 ("Layer 2", float) = .4
		_Color2 ("Color 2", Color) = (.2,.2,.2,1)
		_Level3 ("Layer 3", float) = .6
		_Color3 ("Color 3", Color) = (.4,.4,.4,1)
		_Level4 ("Layer 4", float) = .8
		_Color4 ("Color 4", Color) = (.6,.6,.6,1)
		_PeakColor ("Peak Color", color) = (.8,.8,.8,1)
		_ObjectHeight ("Object Height", float)= 1
	}

	SubShader {
		Tags { "RenderType"="Opaque" } 
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert


		sampler2D _MainTex;

		struct Input {
			float3 customColor;
			float3 worldPos;
			float3 localPos;
		};
		void vert(inout appdata_full v, out Input o) {
			o.customColor = abs(v.normal.y);
			o.localPos = v.vertex.xyz;
		}

		float _Level1;
		float4 _Color1;
		float _Level2;
		float4 _Color2;
		float _Level3;
		float4 _Color3;
		float _Level4;
		float4 _Color4;
		float4 _PeakColor;
		float _ObjectHeight;

		

		void surf (Input IN, inout SurfaceOutput o) {
			if (IN.localPos.y <= (_Level1 - .5) * _ObjectHeight){ 
				o.Albedo = _Color1;
			}
			else if (IN.localPos.y <= (_Level2 - .5) * _ObjectHeight) {
				o.Albedo = _Color2;
			}
			else if (IN.localPos.y <= (_Level3 - .5) * _ObjectHeight) {
				o.Albedo = _Color3;
			}
			else if (IN.localPos.y <= (_Level4 - .5) * _ObjectHeight) {
				o.Albedo = _Color4;
			}
			else {
				o.Albedo = _PeakColor;
			}
		}
		ENDCG
	}
	FallBack "Diffuse"
}
