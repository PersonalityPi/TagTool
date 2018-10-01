using TagTool.Tags.Definitions;

namespace TagTool.Tags.Resources
{
    [TagStructure(Name = "structure_bsp_cache_file_tag_resources", Size = 0x30)]
    public class StructureBspCacheFileTagResources : TagStructure
    {
        [TagField(IsResourceData = true)]
        public TagBlock<ScenarioStructureBsp.UnknownRaw6th> UnknownRaw6ths;
        
        [TagField(IsResourceData = true)]
        public TagBlock<ScenarioStructureBsp.Plane> Planes;
        
        [TagField(IsResourceData = true)]
        public TagBlock<ScenarioStructureBsp.UnknownRaw7th> UnknownRaw7ths;
        
        public TagBlock<PathfindingDatum> PathfindingData;

        [TagStructure(Size = 0x94)]
        public class PathfindingDatum : TagStructure
        {
            [TagField(IsResourceData = true)]
            public TagBlock<ScenarioStructureBsp.PathfindingDatum.Sector> Sectors;

            [TagField(IsResourceData = true)]
            public TagBlock<ScenarioStructureBsp.PathfindingDatum.Link> Links;

            [TagField(IsResourceData = true)]
            public TagBlock<ScenarioStructureBsp.PathfindingDatum.Reference> References;

            [TagField(IsResourceData = true)]
            public TagBlock<ScenarioStructureBsp.PathfindingDatum.Bsp2dNode> Bsp2dNodes;

            [TagField(IsResourceData = true)]
            public TagBlock<ScenarioStructureBsp.PathfindingDatum.Vertex> Vertices;

            public TagBlock<ObjectReference> ObjectReferences;

            [TagField(IsResourceData = true)]
            public TagBlock<ScenarioStructureBsp.PathfindingDatum.PathfindingHint> PathfindingHints;

            [TagField(IsResourceData = true)]
            public TagBlock<ScenarioStructureBsp.PathfindingDatum.InstancedGeometryReference> InstancedGeometryReferences;

            public int StructureChecksum;

            [TagField(IsResourceData = true)]
            public TagBlock<ScenarioStructureBsp.PathfindingDatum.Unknown1Block> Unknown1s;

            public TagBlock<Unknown2Block> Unknown2s;

            public TagBlock<Unknown3Block> Unknown3s;

            [TagField(IsResourceData = true)]
            public TagBlock<ScenarioStructureBsp.PathfindingDatum.Unknown4Block> Unknown4s;

            [TagStructure(Size = 0x18)]
            public class ObjectReference : TagStructure
			{
                public int Unknown;
                public TagBlock<Unknown1Block> Unknown2;
                public int Unknown3;
                public short Unknown4;
                public short Unknown5;

                [TagStructure(Size = 0x18)]
                public class Unknown1Block : TagStructure
				{
                    public sbyte Unknown1;
                    public sbyte Unknown2;
                    public sbyte Unknown3;
                    public sbyte Unknown4;
                    public short Unknown5;
                    public short Unknown6;

                    [TagField(IsResourceData = true)]
                    public TagBlock<ScenarioStructureBsp.PathfindingDatum.ObjectReference.UnknownBlock.UnknownBlock2> Unknown7;

                    public int Unknown8;
                }
            }

            [TagStructure(Size = 0xC)]
            public class Unknown2Block : TagStructure
            {
                [TagField(IsResourceData = true)]
                public TagBlock<ScenarioStructureBsp.PathfindingDatum.Unknown2Block.UnknownBlock> Unknown;
            }

            [TagStructure(Size = 0x14)]
            public class Unknown3Block : TagStructure
			{
                public short Unknown1;
                public short Unknown2;
                public float Unknown3;

                [TagField(IsResourceData = true)]
                public TagBlock<ScenarioStructureBsp.PathfindingDatum.Unknown3Block.UnknownBlock> Unknown4;
            }
        }
    }
}