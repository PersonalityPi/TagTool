using TagTool.Shaders;
using System.Collections.Generic;

namespace TagTool.Tags.Definitions
{
    [TagStructure(Name = "pixel_shader", Tag = "pixl", Size = 0x20)]
    public class PixelShader : TagStructure
	{
        public uint Unknown;
        public TagBlock<ShaderDrawMode> DrawModes;
        public uint Unknown3;
        public TagBlock<PixelShaderBlock> Shaders;
    }
}