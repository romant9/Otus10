// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/UTS_EdgeDetect" { 
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
		_TestColor ("TestColor", Color) = (1,1,1,1)
	}


	CGINCLUDE
	
	#include "UnityCG.cginc"
		
    uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
	uniform float4 _MainTex_TexelSize;
	sampler2D_float _CameraDepthTexture;
	sampler2D _CameraDepthNormalsTexture;

	uniform float4 _Sensitivity;
	uniform float4 _EdgesColor;
	uniform float _Exponent;
	uniform float _SampleDistance;
    uniform float _FilterPower;
    uniform float _Threshold;
	uniform float4 _TestColor;
	uniform float _BgFade;

//Definitions of sructure, Vertex shader and fragment shader for Sobel Color

	float SobelFilter( float dx , float dy , sampler2D tex , float2 uv ){
		float2 delta = float2(dx, dy);
		
		float4 hr = float4(0, 0, 0, 0);
		float4 vt = float4(0, 0, 0, 0);
		
		hr += tex2D(tex, (uv + float2(-1.0, -1.0) * delta)) *  1.0;
		hr += tex2D(tex, (uv + float2( 0.0, -1.0) * delta)) *  0.0;
		hr += tex2D(tex, (uv + float2( 1.0, -1.0) * delta)) * -1.0;
		hr += tex2D(tex, (uv + float2(-1.0,  0.0) * delta)) *  2.0;
		hr += tex2D(tex, (uv + float2( 0.0,  0.0) * delta)) *  0.0;
		hr += tex2D(tex, (uv + float2( 1.0,  0.0) * delta)) * -2.0;
		hr += tex2D(tex, (uv + float2(-1.0,  1.0) * delta)) *  1.0;
		hr += tex2D(tex, (uv + float2( 0.0,  1.0) * delta)) *  0.0;
		hr += tex2D(tex, (uv + float2( 1.0,  1.0) * delta)) * -1.0;
		
		vt += tex2D(tex, (uv + float2(-1.0, -1.0) * delta)) *  1.0;
		vt += tex2D(tex, (uv + float2( 0.0, -1.0) * delta)) *  2.0;
		vt += tex2D(tex, (uv + float2( 1.0, -1.0) * delta)) *  1.0;
		vt += tex2D(tex, (uv + float2(-1.0,  0.0) * delta)) *  0.0;
		vt += tex2D(tex, (uv + float2( 0.0,  0.0) * delta)) *  0.0;
		vt += tex2D(tex, (uv + float2( 1.0,  0.0) * delta)) *  0.0;
		vt += tex2D(tex, (uv + float2(-1.0,  1.0) * delta)) * -1.0;
		vt += tex2D(tex, (uv + float2( 0.0,  1.0) * delta)) * -2.0;
		vt += tex2D(tex, (uv + float2( 1.0,  1.0) * delta)) * -1.0;
		
		return sqrt(hr * hr + vt * vt);
	}

	struct VertInSC {
		float4 vertex : POSITION;
		float2 texcoord0 : TEXCOORD0;
	};
	struct VertOutSC {
		float4 pos : SV_POSITION;
		float2 uv0 : TEXCOORD0;
	};

	VertOutSC vertSC (VertInSC v) {
		VertOutSC
		o = (VertOutSC)0;
		o.uv0 = v.texcoord0;
		o.pos = UnityObjectToClipPos( v.vertex );
		return o;
	}

	float3 HsvToRgb(float3 RGB)
	{
		float3 HSV;

		float minChannel, maxChannel;
		if (RGB.x > RGB.y) {
			maxChannel = RGB.x;
			minChannel = RGB.y;
		}
		else {
			maxChannel = RGB.y;
			minChannel = RGB.x;
		}

		if (RGB.z > maxChannel) maxChannel = RGB.z;
		if (RGB.z < minChannel) minChannel = RGB.z;

		HSV.xy = 0;
		HSV.z = maxChannel;
		float delta = maxChannel - minChannel;             //Delta RGB value
		if (delta != 0) {                    // If gray, leave H  S at zero
			HSV.y = delta / HSV.z;
			float3 delRGB;
			delRGB = (HSV.zzz - RGB + 3 * delta) / (6.0 * delta);
			if (RGB.x == HSV.z) HSV.x = delRGB.z - delRGB.y;
			else if (RGB.y == HSV.z) HSV.x = (1.0 / 3.0) + delRGB.x - delRGB.z;
			else if (RGB.z == HSV.z) HSV.x = (2.0 / 3.0) + delRGB.y - delRGB.x;
		}
		return (HSV);
	}
	/*float3 HsvToRgb(float3 c)
	{
		float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
		float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
		return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
	}*/

	float4 fragSC (VertOutSC i) : COLOR {
		float dx = (_FilterPower/_ScreenParams.r);
		float dy = (_FilterPower/_ScreenParams.g);
		float Edges_value = saturate(lerp(0.0,SobelFilter( dx , dy , _MainTex , i.uv0 ),_Threshold));
		float3 EdgesMask_Var = float3(Edges_value,Edges_value,Edges_value); // EdgesMask		
		float4 SceneColor = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
		SceneColor = lerp(SceneColor, _TestColor, _BgFade);
		//Original
		//float3 FinalColor = saturate(SceneColor.rgb + EdgesMask_Var * _EdgesColor.rgb);
		//float3 FinalColor = saturate(SceneColor.rgb + EdgesMask_Var * _EdgesColor.rgb );// -EdgesMask_Var * _EdgesColor.a);

		//float3 FinalColor = saturate((EdgesMask_Var*(SceneColor.rgb + _EdgesColor.rgb*0.2)) + SceneColor.rgb + SceneColor.rgb * EdgesMask_Var*0.5 - EdgesMask_Var);
		//return float4(FinalColor, 1);
		float3 hsv = HsvToRgb(_EdgesColor.rgb);
		float3 _EdgesColorV = lerp(_EdgesColor.rgb*1.5, SceneColor.rgb*0.5, (1 - hsv.z));
		//float3 _EdgesColorV = lerp(_EdgesColor.rgb * 1.5, SceneColor.rgb - _EdgesColor.rgb, (1 - hsv.z));
		float4 fillColor = float4(lerp(SceneColor.rgb, _EdgesColorV, EdgesMask_Var.xxx * _EdgesColor.a), 1);
		float4 finalColor = float4(lerp(fillColor.rgb, SceneColor.rgb - EdgesMask_Var, 0.2* _EdgesColor.a), 1);
		//return finalColor;
		return float4(finalColor.rgb, SceneColor.a);
	}

//Definitions of Structure, vertex shader and fragement shader fot Sobel Depth and Sobel Depth Thin

	struct v2fd {
		float4 pos : SV_POSITION;
		float2 uv[2] : TEXCOORD0;
	};
		 
	v2fd vertD( appdata_img v )
	{
		v2fd o;
		o.pos = UnityObjectToClipPos (v.vertex);
		
		float2 uv = v.texcoord.xy;
		o.uv[0] = uv;
		
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			uv.y = 1-uv.y;
		#endif
		
		o.uv[1] = uv;
		
		return o;
	}

	float4 fragDCheap(v2fd i) : SV_Target 
	{	
		// inspired by borderlands implementation of popular "sobel filter"

		float centerDepth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv[1]));
		float4 depthsDiag;
		float4 depthsAxis;

		float2 uvDist = _SampleDistance * _MainTex_TexelSize.xy;

		depthsDiag.x = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv[1]+uvDist)); // TR
		depthsDiag.y = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv[1]+uvDist*float2(-1,1))); // TL
		depthsDiag.z = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv[1]-uvDist*float2(-1,1))); // BR
		depthsDiag.w = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv[1]-uvDist)); // BL

		depthsAxis.x = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv[1]+uvDist*float2(0,1))); // T
		depthsAxis.y = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv[1]-uvDist*float2(1,0))); // L
		depthsAxis.z = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv[1]+uvDist*float2(1,0))); // R
		depthsAxis.w = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv[1]-uvDist*float2(0,1))); // B

		depthsDiag -= centerDepth;
		depthsAxis /= centerDepth;

		const float4 HorizDiagCoeff = float4(1,1,-1,-1);
		const float4 VertDiagCoeff = float4(-1,1,-1,1);
		const float4 HorizAxisCoeff = float4(1,0,0,-1);
		const float4 VertAxisCoeff = float4(0,1,-1,0);

		float4 SobelH = depthsDiag * HorizDiagCoeff + depthsAxis * HorizAxisCoeff;
		float4 SobelV = depthsDiag * VertDiagCoeff + depthsAxis * VertAxisCoeff;

		float SobelX = dot(SobelH, float4(1,1,1,1));
		float SobelY = dot(SobelV, float4(1,1,1,1));
		float Sobel = sqrt(SobelX * SobelX + SobelY * SobelY);

		Sobel = 1.0-pow(saturate(Sobel), _Exponent);
		float4 Col = tex2D(_MainTex, i.uv[0].xy);
		Col = _EdgesColor * Col * (1.0 - Sobel) + Sobel;
		return Col * lerp(tex2D(_MainTex, i.uv[0].xy), _TestColor, _BgFade);
	}

	// pretty much also just a sobel filter, except for that edges "outside" the silhouette get discarded
	//  which makes it compatible with other depth based post fx

	float4 fragD(v2fd i) : SV_Target 
	{	
		// inspired by borderlands implementation of popular "sobel filter"

		float centerDepth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv[1]));
		float4 depthsDiag;
		float4 depthsAxis;

		float2 uvDist = _SampleDistance * _MainTex_TexelSize.xy;

		depthsDiag.x = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv[1]+uvDist)); // TR
		depthsDiag.y = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv[1]+uvDist*float2(-1,1))); // TL
		depthsDiag.z = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv[1]-uvDist*float2(-1,1))); // BR
		depthsDiag.w = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv[1]-uvDist)); // BL

		depthsAxis.x = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv[1]+uvDist*float2(0,1))); // T
		depthsAxis.y = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv[1]-uvDist*float2(1,0))); // L
		depthsAxis.z = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv[1]+uvDist*float2(1,0))); // R
		depthsAxis.w = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv[1]-uvDist*float2(0,1))); // B

		// make it work nicely with depth based image effects such as depth of field:
		depthsDiag = (depthsDiag > centerDepth.xxxx) ? depthsDiag : centerDepth.xxxx;
		depthsAxis = (depthsAxis > centerDepth.xxxx) ? depthsAxis : centerDepth.xxxx;

		depthsDiag -= centerDepth;
		depthsAxis /= centerDepth;

		const float4 HorizDiagCoeff = float4(1,1,-1,-1);
		const float4 VertDiagCoeff = float4(-1,1,-1,1);
		const float4 HorizAxisCoeff = float4(1,0,0,-1);
		const float4 VertAxisCoeff = float4(0,1,-1,0);

		float4 SobelH = depthsDiag * HorizDiagCoeff + depthsAxis * HorizAxisCoeff;
		float4 SobelV = depthsDiag * VertDiagCoeff + depthsAxis * VertAxisCoeff;

		float SobelX = dot(SobelH, float4(1,1,1,1));
		float SobelY = dot(SobelV, float4(1,1,1,1));
		float Sobel = sqrt(SobelX * SobelX + SobelY * SobelY);

		Sobel = 1.0-pow(saturate(Sobel), _Exponent);
		//NK
		float4 Col = tex2D(_MainTex, i.uv[0].xy);
		Col = _EdgesColor * Col * (1.0 - Sobel) + Sobel;
		return Col * lerp(tex2D(_MainTex, i.uv[0].xy), _TestColor, _BgFade);
	}

//Definitions of structure, vertex shader and fragment shader for Roberts Cross Depth Normals

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv[5] : TEXCOORD0;
	};

	inline float CheckSame (float2 centerNormal, float centerDepth, float4 theSample)
	{
		// difference in normals
		// do not bother decoding normals - there's no need here
		float2 diff = abs(centerNormal - theSample.xy) * _Sensitivity.y;
		int isSameNormal = (diff.x + diff.y) * _Sensitivity.y < 0.1;
		// difference in depth
		float sampleDepth = DecodeFloatRG (theSample.zw);
		float zdiff = abs(centerDepth-sampleDepth);
		// scale the required threshold by the distance
		int isSameDepth = zdiff * _Sensitivity.x < 0.09 * centerDepth;
	
		// return:
		// 1 - if normals and depth are similar enough
		// 0 - otherwise
		
		return isSameNormal * isSameDepth ? 1.0 : 0.0;
	}	
		
	v2f vertRobert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		
		float2 uv = v.texcoord.xy;
		o.uv[0] = uv;
		
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			uv.y = 1-uv.y;
		#endif
				
		// calc coord for the X pattern
		// maybe nicer TODO for the future: 'rotated triangles'
		
		o.uv[1] = uv + _MainTex_TexelSize.xy * float2(1,1) * _SampleDistance;
		o.uv[2] = uv + _MainTex_TexelSize.xy * float2(-1,-1) * _SampleDistance;
		o.uv[3] = uv + _MainTex_TexelSize.xy * float2(-1,1) * _SampleDistance;
		o.uv[4] = uv + _MainTex_TexelSize.xy * float2(1,-1) * _SampleDistance;
				 
		return o;
	} 

	half4 fragRobert(v2f i) : SV_Target {				
		half4 sample1 = tex2D(_CameraDepthNormalsTexture, i.uv[1].xy);
		half4 sample2 = tex2D(_CameraDepthNormalsTexture, i.uv[2].xy);
		half4 sample3 = tex2D(_CameraDepthNormalsTexture, i.uv[3].xy);
		half4 sample4 = tex2D(_CameraDepthNormalsTexture, i.uv[4].xy);

		half edge = 1.0;
		
		edge *= CheckSame(sample1.xy, DecodeFloatRG(sample1.zw), sample2);
		edge *= CheckSame(sample3.xy, DecodeFloatRG(sample3.zw), sample4);

		//NK
		float4 Col = tex2D(_MainTex, i.uv[0].xy);
		Col = _EdgesColor * Col * (1.0 - edge) + edge;
		return Col * lerp(tex2D(_MainTex, i.uv[0].xy), _TestColor, _BgFade);


		//return edge * lerp(tex2D(_MainTex, i.uv[0]), _BgColor, _BgFade);
	}

	ENDCG 
	
Subshader {
	Tags {
		"IgnoreProjector"="True"
		"Queue"="Overlay+1"
		"RenderType"="Overlay"
	}


//Sobel Depth
 Pass {
	ZTest Always Cull Off ZWrite Off


      CGPROGRAM
	  #pragma target 3.0   
      #pragma vertex vertD
      #pragma fragment fragDCheap
      ENDCG
  }
//Sobel Depth Thin
 Pass {
	ZTest Always Cull Off ZWrite Off


      CGPROGRAM
	  #pragma target 3.0   
      #pragma vertex vertD
      #pragma fragment fragD
      ENDCG
  }

//Roberts Cross Depth Normals
 Pass {
	ZTest Always Cull Off ZWrite Off


      CGPROGRAM
	  #pragma target 3.0
      #pragma vertex vertRobert
      #pragma fragment fragRobert
      ENDCG
  }


//Sobel Color
 Pass {
	ZTest Always Cull Off ZWrite Off


      CGPROGRAM
	  #pragma target 3.0   
      #pragma vertex vertSC
      #pragma fragment fragSC
      ENDCG
  }



}

Fallback off
	
} // shader