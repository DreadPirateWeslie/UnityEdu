Shader "Custom/NewSurfaceShader" {
	Properties{
		_LayerHeights("Layer Heights",float<numberOfLayers>) = <layerHeights>
		_LayerColors("Layer Colors", float<numberOfLayers>) = <layerColors>
		_ObjectHeight("Object Height", float) = <objectHeight>
	}

		SubShader{
		Tags{ "RenderType" = "Opaque" }

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
	float<numberOfLayers - 1> _LayerHeights;
	float<numberOfLayers> _LayerColors;
	float _ObjectHeight;



	void surf(Input IN, inout SurfaceOutput o) {
		if (<isProportional>IN.localPos.y <= (_LayerHeights[0] - .5) * _ObjectHeight
			<isLocal>IN.localPos.y <= _LayerHeights[0]
			<isAbsolue>IN.worldPos.y <= _LayerHeights[0]
			) {
			o.Albedo = _LayerColors[0];
		}

		else if (
			<isProportional>IN.localPos.y <= (_LayerHeights[<l>] - .5) * _ObjectHeight
			<isLocal>IN.localPos.y <= _LayerHeights[<l>]
			<isAbsolue>IN.worldPos.y <= _LayerHeights[<l>]
			) {
			<isNoInterp>o.Albedo = _LayerColors[<l>];
			<isLinearInterp>o.Albedo = lerp(_LayerColors[<l>], _LayerColors[<l> + 1],
				<isLocal> (o.localPos - _LayerHeights[<l>-1]) / (_LayerHeights[<l>] - _LayerHeights[<l>-1]) < isLocal / >
				<isProportional> () / (_LayerHeights[<l>] - _LayerHeights[<l>-1])<isProportional/>
				<isAbsolute> (_LayerHeightsIN.worldPos) / (_LayerHeights[<l>] - _LayerHeights[<l> - 1])<isAbsolute/>
				);<isLinearInterp/>

		}

		else {
			o.Albedo = _LayerColors[_LayerColors.Length - 1];
		}

	}
	ENDCG
	}
		FallBack "Diffuse"
}
