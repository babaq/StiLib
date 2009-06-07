//-----------------------------------------------------------------------------
// File: Grating.fx
//
// Effect Shader to render grating stimulus
//
// Copyright (c) Zhang Li.	2009/03/03
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Effect Parameter definitions
//-----------------------------------------------------------------------------
float4x4 World;
float4x4 View;
float4x4 Projection;

float		time;
float		radius;
float		tf;
float		sf;
float		sphase;
float		sigma;

float4	mincolor;
float4	maxcolor;
float4	colorwidth;

int			PSIndex = 0;
int			VSIndex = 0;
//-----------------------------------------------------------------------------
// Structure definitions
//-----------------------------------------------------------------------------
struct VertexShaderInput
{
    float4 Position	: POSITION0;
    float4 Color		: COLOR0;
};

struct VertexShaderOutput
{
    float4 Position	: POSITION0;
    float4 Color		: COLOR0;
    float2 Texcoord	: TEXCOORD0;
};
//-----------------------------------------------------------------------------
// Compute Grating Color
//-----------------------------------------------------------------------------
float4 Sinusoidal(float4 Position)
{
    // Interpolate Color to Sin Function
    float Cstep = sin( 2 * 3.14159f * sf * ( Position.x + radius + sphase/sf - tf * time ) );
	return float4( (Cstep+1)*colorwidth.x/2+mincolor.x,
						(Cstep+1)*colorwidth.y/2+mincolor.y, 
						(Cstep+1)*colorwidth.z/2+mincolor.z, 
						colorwidth.w );
}

float4 Square(float4 Position)
{
    float Cstep = sin( 2 * 3.14159f * sf * ( Position.x + radius + sphase/sf - tf * time ) );
    if (Cstep>0)
    {
		return float4( maxcolor.x, maxcolor.y, maxcolor.z, colorwidth.w );
	}
	else
	{
		return float4( mincolor.x, mincolor.y, mincolor.z, colorwidth.w );
	}
}

float4 Linear(float4 Position)
{
	float X = abs( Position.x + radius + sphase/sf - tf * time );
	float XX = fmod( X, 1/sf/2 );
	float which = - sign( fmod( floor( X * sf * 2 ), 2 ) - 0.5 );
	float Cstep= which * 4 * sf * XX;
	if ( abs(Cstep) > 1 )
	{
		Cstep = which * 2 - Cstep;
	}
	return float4( (Cstep+1)*colorwidth.x/2+mincolor.x,
						(Cstep+1)*colorwidth.y/2+mincolor.y, 
						(Cstep+1)*colorwidth.z/2+mincolor.z, 
						colorwidth.w );				 
}
//-----------------------------------------------------------------------------
// Vertex shaders
//-----------------------------------------------------------------------------
VertexShaderOutput VSSin(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    
    output.Color = Sinusoidal(input.Position);
	
	output.Texcoord.x = input.Position.x;
	output.Texcoord.y = input.Position.y;
	
    return output;
}

VertexShaderOutput VSSquare(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    
	output.Color = Square(input.Position);
								 
	output.Texcoord.x = input.Position.x;
	output.Texcoord.y = input.Position.y;
	
    return output;
}

VertexShaderOutput VSLinear(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    
	output.Color = Linear(input.Position);
				 
	output.Texcoord.x = input.Position.x;
	output.Texcoord.y = input.Position.y;
	
    return output;
}
//-----------------------------------------------------------------------------
// Pixel shaders
//-----------------------------------------------------------------------------
float4 PSNone(VertexShaderOutput input) : COLOR0
{
    return input.Color;
}

float4 PSGaussian(VertexShaderOutput input) : COLOR0
{
	float d = pow( input.Texcoord.x, 2 ) + pow( input.Texcoord.y, 2 );
    return input.Color * exp( - d / (2 * pow(sigma, 2) ) );
}
//-----------------------------------------------------------------------------
// Shader and technique definitions
//-----------------------------------------------------------------------------
VertexShader VSArray[3] =
{
	compile vs_2_0 VSSin(),
	compile vs_2_0 VSSquare(),
	compile vs_2_0 VSLinear(),
};

PixelShader PSArray[2] =
{
	compile ps_2_0 PSNone(),
	compile ps_2_0 PSGaussian(),
};

technique Technique1
{
    pass Pass1
    {
        VertexShader = (VSArray[VSIndex]);
        PixelShader = (PSArray[PSIndex]);
    }
}
