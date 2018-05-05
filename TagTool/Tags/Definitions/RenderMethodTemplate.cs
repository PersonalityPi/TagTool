using TagTool.Cache;
using TagTool.Common;
using TagTool.Serialization;
using System.Collections.Generic;

namespace TagTool.Tags.Definitions
{
    [TagStructure(Name = "render_method_template", Tag = "rmt2", Size = 0x84, MaxVersion = CacheVersion.Halo3ODST)]
    [TagStructure(Name = "render_method_template", Tag = "rmt2", Size = 0x90, MinVersion = CacheVersion.HaloOnline106708)]
    public class RenderMethodTemplate
    {
        public enum ShaderMode : sbyte
        {
            Default,
            Albedo,
            Static_Default,
            Static_Per_Pixel,
            Static_Per_Vertex,
            Static_Sh,
            Static_Prt_Ambient,
            Static_Prt_Linear,
            Static_Prt_Quadratic,
            Dynamic_Light,
            Shadow_Generate,
            Shadow_Apply,
            Active_Camo,
            Lightmap_Debug_Mode,
            Static_Per_Vertex_Color,
            Water_Tessellation,
            Water_Shading,
            Dynamic_Light_Cinematic,
            Z_Only,
            Sfx_Distort
        }

        public enum ShaderModeBitmask : uint
        {
            Default = 1 << 0,
            Albedo = 1 << 1,
            Static_Default = 1 << 2,
            Static_Per_Pixel = 1 << 3,
            Static_Per_Vertex = 1 << 4,
            Static_Sh = 1 << 5,
            Static_Prt_Ambient = 1 << 6,
            Static_Prt_Linear = 1 << 7,
            Static_Prt_Quadratic = 1 << 8,
            Dynamic_Light = 1 << 9,
            Shadow_Generate = 1 << 10,
            Shadow_Apply = 1 << 11,
            Active_Camo = 1 << 12,
            Lightmap_Debug_Mode = 1 << 13,
            Static_Per_Vertex_Color = 1 << 14,
            Water_Tessellation = 1 << 15,
            Water_Shading = 1 << 16,
            Dynamic_Light_Cinematic = 1 << 17,
            Z_Only = 1 << 18,
            Sfx_Distort = 1 << 19,
        }

        public CachedTagInstance VertexShader;
        public CachedTagInstance PixelShader;
        public ShaderModeBitmask DrawModeBitmask;
        public List<RMT2PackedUInt16> DrawModes; // Entries in here correspond to an enum in the EXE
        public List<DrawModeRegisterOffsetBlock> DrawModeRegisterOffsets;
        public List<ArgumentMapping> ArgumentMappings;
        public List<ShaderArgument> Arguments;
        public List<ShaderArgument> Unknown5;
        public List<ShaderArgument> GlobalArguments;
        public List<ShaderArgument> ShaderMaps;
        public uint Unknown7;
        public uint Unknown8;
        public uint Unknown9;

        [TagField(Padding = true, Length = 12, MinVersion = CacheVersion.HaloOnline106708)]
        public byte[] Unused;

        [TagStructure(Size = 0x2)]
        public class RMT2PackedUInt16 : PackedIntegerBase
        {
            public ushort Value;

            public ushort Offset { get => GetValue(0, 10); set => SetValue(0, 10, value); }
            public ushort Count { get => GetValue(10, 6); set => SetValue(10, 6, value); }
        }

        [TagStructure(Size = 0x1C)]
        public class DrawModeRegisterOffsetBlock
        {
            RMT2PackedUInt16 Textures_Samplers;
            RMT2PackedUInt16 Unknown_Vectors;
            RMT2PackedUInt16 Unidentified_RegsiterOffset1;
            RMT2PackedUInt16 Unidentified_RegsiterOffset2;
            RMT2PackedUInt16 Arguments_Vectors;
            RMT2PackedUInt16 Unidentified_RegsiterOffset3;
            RMT2PackedUInt16 Global_Arguments;
            RMT2PackedUInt16 Render_Method_Extern_Generic;
            RMT2PackedUInt16 Unidentified_RegsiterOffset4;
            RMT2PackedUInt16 Unidentified_RegsiterOffset5;
            RMT2PackedUInt16 Render_Method_Extern_Vectors;
            RMT2PackedUInt16 Unidentified_RegsiterOffset6;
            RMT2PackedUInt16 Unidentified_RegsiterOffset7;
            RMT2PackedUInt16 Unidentified_RegsiterOffset8;
        }

        /// <summary>
        /// Binds an argument in the render method tag to a pixel shader constant.
        /// </summary>
        [TagStructure(Size = 0x4)]
        public class ArgumentMapping
        {
            /// <summary>
            /// The GPU register to bind the argument to.
            /// </summary>
            public ushort RegisterIndex;

            /// <summary>
            /// The index of the argument in one of the blocks in the render method tag.
            /// The block used depends on the argument type.
            /// </summary>
            public byte ArgumentIndex;

            public byte Unknown;
        }

        [TagStructure(Size = 0x4)]
        public class ShaderArgument
        {
            public StringId Name;
        }
    }
}