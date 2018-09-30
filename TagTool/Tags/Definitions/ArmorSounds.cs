using TagTool.Cache;
using TagTool.Common;
using System.Collections.Generic;

namespace TagTool.Tags.Definitions
{
    [TagStructure(Name = "armor_sounds", Tag = "arms", Size = 0x10, MinVersion = CacheVersion.HaloOnline106708)]
    public class ArmorSounds : TagStructure
	{
        public TagBlock<ArmorSound> ArmorSounds2;
        public uint Unknown;

        [TagStructure(Size = 0x24)]
        public class ArmorSound : TagStructure
		{
            public TagBlock<TagReferenceBlock> Unknown1;
            public TagBlock<TagReferenceBlock> Unknown2;
            public TagBlock<TagReferenceBlock> Unknown3;
        }
    }
}