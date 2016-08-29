Shader "Custom/Fixed Unlit"
{
	Properties
	{
		_Opacity("Opacity", Range(0, 1)) = 1
		_MainTex("Main Texture", 2D) = "white" {}
	}
		SubShader
	{
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

		struct appdata
	{
		float4 vertex : POSITION;
		float2 texCoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		float2 texCoord : TEXCOORD0;
	};

	float _Opacity;
	sampler2D _MainTex;

	v2f vert(appdata IN)
	{
		v2f OUT;
		OUT.pos = mul(UNITY_MATRIX_MVP, IN.vertex);
		OUT.texCoord = IN.texCoord;
		return OUT;
	}

	fixed4 frag(v2f IN) : COLOR
	{
		fixed4 texColor = tex2D(_MainTex, IN.texCoord);
		return texColor;
	}

		ENDCG
	}
	}
}