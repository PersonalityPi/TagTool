using TagTool.Cache;
using TagTool.Common;
using TagTool.Serialization;
using System.Collections.Generic;

namespace TagTool.Tags.Definitions
{
    [TagStructure(Name = "camera_fx_settings", Tag = "cfxs", Size = 0xE4, MaxVersion = CacheVersion.Halo3ODST)]
    [TagStructure(Name = "camera_fx_settings", Tag = "cfxs", Size = 0x170, MinVersion = CacheVersion.HaloOnline106708)]
    public class CameraFxSettings
    {
        public ushort Flags;
        public short Unknown;
        public float OverexposureAmount;
        public float OverexposureUnknown;
        public float OverexposureUnknown2;
        public float BrightnessAmount;
        public float BrightnessUnknown;
        public float BrightnessUnknown2;
        public float BrightnessUnknown3;
        public ushort Flags2;
        public short Unknown2;
        public float Unknown3;
        public ushort Flags3;
        public short Unknown4;
        public float Unknown5;
        public ushort Flags4;
        public short Unknown6;
        public float Base;
        public float Min;
        public float Max;
        public ushort Flags5;
        public short Unknown7;
        public float Base2;
        public float Min2;
        public float Max2;
        public ushort Flags6;
        public short Unknown8;
        public float Base3;
        public float Min3;
        public float Max3;
        public ushort Flags7;
        public short Unknown9;
        public float Red;
        public float Green;
        public float Blue;
        public ushort Flags8;
        public short Unknown10;
        public float Red2;
        public float Green2;
        public float Blue2;
        public ushort Flags9;
        public short Unknown11;
        public float Red3;
        public float Green3;
        public float Blue3;
        public ushort Flags10;
        public short Unknown12;
        public float Unknown13;
        public float Unknown14;
        public float Unknown15;
        public ushort Flags11;
        public short Unknown16;
        public float Unknown17;
        public float Unknown18;
        public float Unknown19;
        public ushort Flags12;
        public short Unknown20;
        public float Unknown21;
        public float Unknown22;
        public float Unknown23;
        public int Unknown24;
        public ushort Flags13;
        public short Unknown25;
        public float Base4;
        public float Min4;
        public float Max4;
        public ushort Flags14;
        public short Unknown26;
        public float Base5;
        public float Min5;
        public float Max5;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public ushort Flags15;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public short Unknown27;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public float Unknown28;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public float Unknown29;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public float Unknown30;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public ushort Flags16;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public short Unknown31;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public float Unknown32;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public List<UnknownBlock> Unknown33;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public List<UnknownBlock2> Unknown34;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public List<UnknownBlock3> Unknown35;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public List<UnknownBlock4> Unknown36;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public List<UnknownBlock5> Unknown37;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public List<UnknownBlock6> Unknown38;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public ushort Flags17;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public short Unknown39;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public float Unknown40;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public float Unknown41;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public float ColorR;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public float ColorG;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public float ColorB;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public float Unknown42;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public float Unknown43;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public float Unknown44;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public float Unknown45;
        [TagField(MinVersion = CacheVersion.HaloOnline106708)]
        public float Unknown46;

        [TagStructure(Size = 0x58)]
        public class UnknownBlock
        {
            public float Unknown;
            public int Unknown2;
            public TagFunction Unknown3 = new TagFunction { Data = new byte[0] };
            public TagFunction Unknown4 = new TagFunction { Data = new byte[0] };
            public TagFunction Unknown5 = new TagFunction { Data = new byte[0] };
            public TagFunction Unknown6 = new TagFunction { Data = new byte[0] };
        }

        [TagStructure(Size = 0xC)]
        public class UnknownBlock2
        {
            public int Unknown;
            public float Unknown2;
            public float Unknown3;
        }

        [TagStructure(Size = 0x14)]
        public class UnknownBlock3
        {
            public float Unknown;
            public float Unknown2;
            public float Unknown3;
            public float Unknown4;
            public float Unknown5;
        }

        [TagStructure(Size = 0x14)]
        public class UnknownBlock4
        {
            public float Unknown;
            public float Unknown2;
            public float Unknown3;
            public float Unknown4;
            public float Unknown5;
        }

        [TagStructure(Size = 0x94)]
        public class UnknownBlock5
        {
            public int Unknown;
            public float Unknown2;
            public float Unknown3;
            public float Unknown4;
            public float Unknown5;
            public float Unknown6;
            public float Unknown7;
            public float Unknown8;
            public float Unknown9;
            public float Unknown10;
            public float Unknown11;
            public float Unknown12;
            public float Unknown13;
            public float Unknown14;
            public float Unknown15;
            public float Unknown16;
            public float Unknown17;
            public float Unknown18;
            public float Unknown19;
            public float Unknown20;
            public float Unknown21;
            public float Unknown22;
            public float Unknown23;
            public float Unknown24;
            public float Unknown25;
            public float Unknown26;
            public float Unknown27;
            public float Unknown28;
            public float Unknown29;
            public float Unknown30;
            public float Unknown31;
            public float Unknown32;
            public float Unknown33;
            public float Unknown34;
            public float Unknown35;
            public float Unknown36;
            public float Unknown37;
        }

        [TagStructure(Size = 0x28)]
        public class UnknownBlock6
        {
            public int Unknown;
            public float Unknown2;
            public float Unknown3;
            public float Unknown4;
            public float Unknown5;
            public float Unknown6;
            public float Unknown7;
            public float Unknown8;
            public float Unknown9;
            public float Unknown10;
        }
    }
}