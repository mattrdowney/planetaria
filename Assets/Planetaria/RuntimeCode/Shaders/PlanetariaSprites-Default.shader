// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt) - with modifications

Shader "Planetaria/PlanetariaSprites/Default"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting On
		ZWrite Off
		Blend One OneMinusSrcAlpha

		CGPROGRAM
#pragma surface surface_shader TwoDimensional vertex:vertex_shader nofog nolightmap nodynlightmap keepalpha noinstancing
#pragma multi_compile _ PIXELSNAP_ON
#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
#include "UnitySprites.cginc"

			half4 LightingTwoDimensional(SurfaceOutput surface, half3 light_direction, half attenuation)
			{
				half light_value = sign(dot(surface.Normal, light_direction))*0.5 + 0.5; // NOTE: sign can return -1, +1, OR 0 (intended)
				half4 result;
				result.rgb = surface.Albedo * _LightColor0.rgb * light_value * attenuation;
				result.a = surface.Alpha;
				return result;
			}

			struct Input
			{
				float2 uv_MainTex;
				fixed4 color;
			};

			void vertex_shader(inout appdata_full v, out Input o)
			{
				v.vertex = UnityFlipSprite(v.vertex, _Flip);

#if defined(PIXELSNAP_ON)
				v.vertex = UnityPixelSnap(v.vertex);
#endif

				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.color = v.color * _Color * _RendererColor;
			}

			void surface_shader(Input IN, inout SurfaceOutput o)
			{
				fixed4 c = SampleSpriteTexture(IN.uv_MainTex) * IN.color;
				o.Albedo = c.rgb * c.a;
				o.Alpha = c.a;
			}
		ENDCG
	}

	Fallback "Transparent/VertexLit"
}