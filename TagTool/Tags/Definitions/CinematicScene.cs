using TagTool.Cache;
using TagTool.Common;
using System.Collections.Generic;
using System;

namespace TagTool.Tags.Definitions
{
    [TagStructure(Name = "cinematic_scene", Tag = "cisc", Size = 0x78, MinVersion = CacheVersion.Halo3Retail)]
    public class CinematicScene : TagStructure
	{
        public StringId Name;

        [TagField(Length = 32)]
        public string AnchorName;

        public uint Unknown1;
        public byte[] ImportScript1;

        public TagBlock<PuppetBlock> Puppets;
        public TagBlock<ShotBlock> Shots;
        public TagBlock<TextureCameraBlock> TextureCameras;

        public byte[] importScript2;
        public uint Unknown3;
        
        [TagStructure(Size = 0x74)]
        public class PuppetBlock : TagStructure
		{
            [TagField(Length = 32)]
            public string ImportName;

            [TagField(Label = true)]
            public StringId Name;
            public StringId Variant;
            public CachedTagInstance PuppetAnimation;
            public CachedTagInstance PuppetObject;

            public uint Unknown1;
            public byte Unknown2;
            public byte Unknown3;
            public byte Unknown4;
            public byte Unknown5;
            public int Unknown6;

            public byte[] ImportScript;

            public TagBlock<UnknownBlock> Unknown7;

            [TagStructure(Size = 0x38)]
            public class UnknownBlock : TagStructure
			{
                public uint Unknown1;
                public uint Unknown2;
                public uint Unknown3;
                public uint Unknown4;
                public uint Unknown5;
                public uint Unknown6;
                public uint Unknown7;
                public uint Unknown8;
                public uint Unknown9;
                public uint Unknown10;
                public CachedTagInstance Unknown11;
            }
        }

        [TagStructure(Size = 0xA4, MaxVersion = CacheVersion.Halo3Retail)]
        [TagStructure(Size = 0xBC, MinVersion = CacheVersion.Halo3ODST)]
        public class ShotBlock : TagStructure
		{
            public byte[] OpeningImportScripts;
            public int Unknown1;
            public uint Unknown2;
            public float Unknown3;
            public TagBlock<LightingBlock> Lighting;
            public TagBlock<UnknownBlock> Unknown4;
            public TagBlock<SoundBlock> Sounds;
            public TagBlock<BackgroundSoundBlock> BackgroundSounds;
            public TagBlock<EffectBlock> Effects;
            public TagBlock<FunctionBlock> Functions;

            [TagField(MinVersion = CacheVersion.Halo3ODST)]
            public TagBlock<ScreenEffectBlock> ScreenEffects;

            public TagBlock<CortanaEffectBlock> CortanaEffects;
            public TagBlock<ImportScriptBlock> ImportScripts;

            //Might be an extra unused block
            [TagField(MinVersion = CacheVersion.Halo3ODST)]
            public uint Unknown5;
            [TagField(MinVersion = CacheVersion.Halo3ODST)]
            public uint Unknown6;
            [TagField(MinVersion = CacheVersion.Halo3ODST)]
            public uint Unknown7;

            public byte[] ImportScript1;
            public int LoadedFrameCount;
            public TagBlock<FrameBlock> Frames;

            [TagStructure(Size = 0x18, MaxVersion = CacheVersion.Halo3Retail)]
            [TagStructure(Size = 0x1C, MinVersion = CacheVersion.Halo3ODST)]
            public class LightingBlock : TagStructure
			{
                [TagField(MinVersion = CacheVersion.Halo3ODST)]
                public uint Unknown;

                [TagField(Label = true)]
                public CachedTagInstance CinematicLight;
                public int OwnerPuppetIndex;
                public StringId Marker;
            }

            [TagStructure(Size = 0x2C)]
            public class UnknownBlock : TagStructure
			{
                public uint Unknown1;
                public uint Unknown2;
                public uint Unknown3;
                public uint Unknown4;
                public uint Unknown5;
                public uint Unknown6;
                public uint Unknown7;
                public uint Unknown8;

                public TagBlock<UnknownBlock2> Unknown9;

                [TagStructure(Size =0x4)]
                public class UnknownBlock2 : TagStructure
				{
                    public uint Unknown;
                }

            }

            [TagStructure(Size = 0x24)]
            public class SoundBlock : TagStructure
			{
                [TagField(Label = true)]
                public CachedTagInstance Sound;
                public int Frame;
                public float Unknown1;
                public StringId Unknown2;
                public uint Unknown3;
                public StringId Unknown4;
            }

            [TagStructure(Size = 0x18)]
            public class BackgroundSoundBlock : TagStructure
			{
                public uint Unknown1;
                [TagField(Label = true)]
                public CachedTagInstance Sound;
                public int Frame;
            }

            [TagStructure(Size = 0x1C)]
            public class EffectBlock : TagStructure
			{
                [TagField(Label = true)]
                public CachedTagInstance Effect;
                public int Frame;
                public StringId Marker;
                public int OwnerPuppetIndex;
            }

            [TagStructure(Size = 0x14)]
            public class FunctionBlock : TagStructure
			{
                public int OwnerPuppetIndex;
                [TagField(Label = true)]
                public StringId TargetFunctionName;
                public TagBlock<UnknownBlock2> Unknown;

                [TagStructure(Size = 0x10)]
                public class UnknownBlock2 : TagStructure
				{
                    public uint Unknown1;
                    public int Unknown2;
                    public float Unknown3;
                    public float Unknown4;
                }
            }

            [TagStructure(Size = 0x18)]
            public class ScreenEffectBlock : TagStructure
			{
                [TagField(Label = true)]
                public CachedTagInstance Effect;
                public int StartFrame;
                public int EndFrame;
            }

            [TagStructure(Size = 0x14)]
            public class CortanaEffectBlock : TagStructure
			{
                [TagField(Label = true)]
                public CachedTagInstance Effect;
                public uint Unknown;
            }

            [TagStructure(Size = 0x18)]
            public class ImportScriptBlock : TagStructure
			{
                public int Frame;
                public byte[] ImportScript;
            }

            [TagStructure(Size = 0x44)]
            public class FrameBlock : TagStructure
			{
                public RealPoint3d Position;
                public float Unknown1;
                public float Unknown2;
                public float Unknown3;
                public float Unknown4;
                public float Unknown5;
                public float Unknown6;
                public float Unknown7;
                public float Unknown8;
                public float FOV;

                //Depth of field options

                public FlagBits Flags;
                public float NearPlane;
                public float FarPlane;
                public float FocalDepth;
                public float BlurAmount;

                [Flags]
                public enum FlagBits : int
                {
                    None,
                    EnableDepthOfField = 1 << 0
                }
            }
        }

        [TagStructure(Size = 0xC)]
        public class TextureCameraBlock : TagStructure
		{
            [TagField(Label = true)]
            public StringId Name;
            public StringId Unknown;
            public TagBlock<CameraShotBlock> Shots;

            [TagStructure(Size = 0xC)]
            public class CameraShotBlock : TagStructure
			{
                public TagBlock<FrameBlock> Frames;

                [TagStructure(Size = 0x48)]
                public class FrameBlock : TagStructure
				{
                    public uint UnknownIndex;
                    public RealPoint3d Position;
                    public float Unknown1;
                    public float Unknown2;
                    public float Unknown3;
                    public float Unknown4;
                    public float Unknown5;
                    public float Unknown6;
                    public float Unknown7;
                    public float Unknown8;
                    public float FOV;

                    //Depth of field options

                    public int Flags;
                    public float NearPlane;
                    public float FarPlane;
                    public float FocalDepth;
                    public float BlurAmount;
                }
            }
        }
    }
}