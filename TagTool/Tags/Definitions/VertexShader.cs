using TagTool.Cache;
using TagTool.Shaders;
using System.Collections.Generic;

namespace TagTool.Tags.Definitions
{
    [TagStructure(Name = "vertex_shader", Tag = "vtsh", Size = 0x20, MinVersion = CacheVersion.Halo3Retail)]
    public class VertexShader : TagStructure
	{
        public uint Unknown;
        public TagBlock<DrawModeList> DrawModeLists;
        public uint Unknown3;
        public TagBlock<VertexShaderBlock> Shaders;

        [TagStructure(Size = 0xC)]
        public class DrawModeList : TagStructure
		{
            public TagBlock<ShaderDrawMode> DrawModes;
        }
    }
}