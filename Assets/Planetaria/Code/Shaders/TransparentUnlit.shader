Shader "Planetaria/Transparent Unlit"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" } // CONSIDER: transparent cutout should almost definitely be used
		LOD 100
		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surface_shader TwoDimensional alpha

		half4 LightingTwoDimensional(SurfaceOutput surface, half3 light_direction, half attenuation)
		{
			half4 result;
			result.rgb = surface.Albedo * _LightColor0.rgb * attenuation;
			result.a = surface.Alpha;
			return result;
		}

		struct Input
		{
			float2 uv_MainTex;
		};

		sampler2D _MainTex;

		void surface_shader(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}

/*
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/