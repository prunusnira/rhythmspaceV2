// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Chroma Key Colored"
{
	Properties
	{
		_Color("Main Color", Color) = (0,0,0,1)
		_MainTex("Base (RGB), Alpha (A)", 2D) = "black" {}
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset 0, 0
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			half4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			v2f o;

			v2f vert(appdata_t v)
			{
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;

				return o;
			}

			fixed4 frag(v2f IN) : COLOR
			{
				half4 c = tex2D(_MainTex, IN.texcoord) * IN.color;

				if (c.r == _Color.r && c.g == _Color.g && c.b == _Color.b)
				{
					c.a = 0;
				}
				/*if (c.g != _Color.g && c.r != _Color.r && c.b != _Color.b)
				{
					c.a = c.a;
				}
				else
				{
					c.a = 0;
				}*/

				return c;
			}
			ENDCG
		}
	}

	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset 0, 0
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse

			SetTexture[_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}
}
