Shader "Unlit/UI-GaussianBlur"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_BlurSize ("Blur Size", Float) = 1.0
	}
	SubShader
	{
		Tags 
		{ 
			"Queue"="Transparent"
			"RenderType"="Transparent" 
		}
		LOD 100
		GrabPass
		{
			"_BackgroundTexture"
		}
		Pass
		{
			Name "Blur_Vertical"
			CGPROGRAM
			#pragma vertex vertBlurVertical
			#pragma fragment fragBlur
			ENDCG
		}
		Pass
		{
			Name "Blur_Horizontal"
			CGPROGRAM
			#pragma vertex vertBlurHorizontal
			#pragma fragment fragBlur
			ENDCG
		}

	CGINCLUDE
		#include "UnityCG.cginc"
		sampler2D _BackgroundTexture;
		float4 _BackgroundTexture_TexelSize;
		float _BlurSize;

		struct appdata_t {
			float4 vertex 		: POSITION;
			float2 texcoord		: TEXCOORD0;
			float4 color    	: COLOR;
		};

		struct VertexOutput_Blur {
			float4 pos 				: SV_POSITION;
			float4 grabPos[5]		: TEXCOORD0;
		};

		VertexOutput_Blur vertBlurVertical(appdata_t v) {
			VertexOutput_Blur o;
			o.pos = UnityObjectToClipPos(v.vertex);

			float4 grabPos = ComputeGrabScreenPos(o.pos);
			o.grabPos[0] = grabPos;
			o.grabPos[1] = grabPos + float4(0.0, _BackgroundTexture_TexelSize.y * 1.0, 0.0, 0.0) * _BlurSize;
			o.grabPos[2] = grabPos - float4(0.0, _BackgroundTexture_TexelSize.y * 1.0, 0.0, 0.0) * _BlurSize;
			o.grabPos[3] = grabPos + float4(0.0, _BackgroundTexture_TexelSize.y * 2.0, 0.0, 0.0) * _BlurSize;
			o.grabPos[4] = grabPos - float4(0.0, _BackgroundTexture_TexelSize.y * 2.0, 0.0, 0.0) * _BlurSize;

			return o;
		}

		VertexOutput_Blur vertBlurHorizontal(appdata_t v) {
			VertexOutput_Blur o;
			o.pos = UnityObjectToClipPos(v.vertex);

			float4 grabPos = ComputeGrabScreenPos(o.pos);
			o.grabPos[0] = grabPos;
			o.grabPos[1] = grabPos + float4(_BackgroundTexture_TexelSize.x * 1.0, 0.0, 0.0, 0.0) * _BlurSize;
			o.grabPos[2] = grabPos - float4(_BackgroundTexture_TexelSize.x * 1.0, 0.0, 0.0, 0.0) * _BlurSize;
			o.grabPos[3] = grabPos + float4(_BackgroundTexture_TexelSize.x * 2.0, 0.0, 0.0, 0.0) * _BlurSize;
			o.grabPos[4] = grabPos - float4(_BackgroundTexture_TexelSize.x * 2.0, 0.0, 0.0, 0.0) * _BlurSize;

			return o;
		}


		fixed4 fragBlur(VertexOutput_Blur i) : SV_Target
		{
			float weight[3] = {0.4026, 0.2442, 0.0545};
			//float weight[3] = {0.0, 0.25, 0.25};

			fixed3 sum = tex2Dproj(_BackgroundTexture, i.grabPos[0]).rgb * weight[0];
			sum += tex2Dproj(_BackgroundTexture, i.grabPos[1]).rgb * weight[1];
			sum += tex2Dproj(_BackgroundTexture, i.grabPos[2]).rgb * weight[1];
			sum += tex2Dproj(_BackgroundTexture, i.grabPos[3]).rgb * weight[2];
			sum += tex2Dproj(_BackgroundTexture, i.grabPos[4]).rgb * weight[2];

			return fixed4(sum, 1.0);
		}
	ENDCG
	}
}
