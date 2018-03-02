cbuffer CameraBuffer :register(b0)
{
	float4x4 worldViewProj;
}; 

Texture2D textureMap;
SamplerState textureSampler
{
	
};

struct VS_IN
{
	float4 pos : POSITION;
	float4 tc : TEXCOORD;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 tc : TEXCOORD;
};

PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;

	//output.pos = input.pos;
	output.pos = mul(worldViewProj, input.pos);
	output.tc = input.tc;

	return output;
}

float4 PS(PS_IN input) : SV_Target
{
	//return input.pos;
	return textureMap.Sample(textureSampler, input.tc);
}