using TagTool.Shaders;
using System.Collections.Generic;

namespace TagTool.Tags.Definitions
{
    [TagStructure(Name = "global_vertex_shader", Tag = "glvs", Size = 0x1C)]
    public class GlobalVertexShader : TagStructure
	{
        public TagBlock<VertexTypeShaders> VertexTypes;
        public uint Unknown2;
        public TagBlock<VertexShaderBlock> Shaders;

        [TagStructure(Size = 0xC)]
        public class VertexTypeShaders : TagStructure
		{
            public TagBlock<DrawMode> DrawModes;

            [TagStructure(Size = 0x10)]
            public class DrawMode : TagStructure
			{
                public uint Unknown;
                public uint Unknown2;
                public uint Unknown3;
                public int ShaderIndex;
            }
        }
    }
}
