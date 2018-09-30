using TagTool.Common;
using TagTool.Tags;
using System.Collections.Generic;

namespace TagTool.Shaders
{
    [TagStructure(Size = 0x50)]
    public class ShaderData : TagStructure
	{
        public byte[] Unknown1;
        public byte[] PcCompiledShader;
        public TagBlock<ShaderParameter> XboxParameters;
        public uint Unknown2;
        public TagBlock<ShaderParameter> PcParameters;
        public StringId Unknown3;
        public uint Unknown4;
        public uint XboxShaderAddress;
    }
}