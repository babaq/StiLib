//-----------------------------------------------------------------------------
// File: HardwareInstancing.fx
//
// Effect Shader to Render Vision Stimulus Collection Using Hardware Instancing
//
// Copyright (c) Zhang Li.	2009/08/10.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Effect Parameter definitions
//-----------------------------------------------------------------------------
float4x4 World;
float4x4 View;
float4x4 Projection;
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
};
//-----------------------------------------------------------------------------
// Vertex shaders
//-----------------------------------------------------------------------------
VertexShaderOutput VertexShaderFunction(VertexShaderInput input, float4x4 instanceTransform : TEXCOORD1)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, transpose(instanceTransform));
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    
    output.Color = input.Color;
    
    return output;
}
//-----------------------------------------------------------------------------
// Pixel shaders
//-----------------------------------------------------------------------------
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return input.Color;
}
//-----------------------------------------------------------------------------
// Shader and technique definitions
//-----------------------------------------------------------------------------
technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}