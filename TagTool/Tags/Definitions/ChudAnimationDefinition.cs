using TagTool.Cache;
using TagTool.Common;
using System.Collections.Generic;

namespace TagTool.Tags.Definitions
{
    [TagStructure(Name = "chud_animation_definition", Tag = "chad", Size = 0x5C)]
    public class ChudAnimationDefinition : TagStructure
	{
        public ushort Flags;
        public short Unknown;
        public TagBlock<PositionBlock> Position;
        public TagBlock<RotationBlock> Rotation;
        public TagBlock<SizeBlock> Size;
        public TagBlock<ColorBlock> Color;
        public TagBlock<AlphaBlock> Alpha;
        public TagBlock<AlphaUnknownBlock> AlphaUnknown;
        public TagBlock<BitmapBlock> Bitmap;
        public int NumberOfFrames;

        [TagStructure(Size = 0x20)]
        public class PositionBlock : TagStructure
		{
            public TagBlock<AnimationBlock> Animation;
            public TagFunction Function = new TagFunction { Data = new byte[0] };

            [TagStructure(Size = 0x10)]
            public class AnimationBlock : TagStructure
			{
                public int FrameNumber;
                public RealPoint3d Position;
            }
        }

        [TagStructure(Size = 0x20)]
        public class RotationBlock : TagStructure
		{
            public TagBlock<AnimationBlock> Animation;
            public TagFunction Function = new TagFunction { Data = new byte[0] };

            [TagStructure(Size = 0x10)]
            public class AnimationBlock : TagStructure
			{
                public int FrameNumber;
                public RealEulerAngles3d Angles;
            }
        }

        [TagStructure(Size = 0x20)]
        public class SizeBlock : TagStructure
		{
            public TagBlock<AnimationBlock> Animation;
            public TagFunction Function = new TagFunction { Data = new byte[0] };

            [TagStructure(Size = 0xC)]
            public class AnimationBlock : TagStructure
			{
                public int FrameNumber;
                public RealPoint2d Stretch;
            }
        }

        [TagStructure(Size = 0x20)]
        public class ColorBlock : TagStructure
		{
            public TagBlock<AnimationBlock> Animation;
            public TagFunction Function = new TagFunction { Data = new byte[0] };

            [TagStructure(Size = 0x4, MaxVersion = CacheVersion.Halo3ODST)]
            [TagStructure(Size = 0x8, MinVersion = CacheVersion.HaloOnline106708)]
            public class AnimationBlock : TagStructure
			{
                public int FrameNumber;
                [TagField(MinVersion = CacheVersion.HaloOnline106708)]
                public uint Unknown;
            }
        }

        [TagStructure(Size = 0x20)]
        public class AlphaBlock : TagStructure
		{
            public TagBlock<AnimationBlock> Animation;
            public TagFunction Function = new TagFunction { Data = new byte[0] };

            [TagStructure(Size = 0x8)]
            public class AnimationBlock : TagStructure
			{
                public int FrameNumber;
                public float Alpha;
            }
        }

        [TagStructure(Size = 0x20)]
        public class AlphaUnknownBlock : TagStructure
		{
            public TagBlock<AnimationBlock> Animation;
            public TagFunction Function = new TagFunction { Data = new byte[0] };

            [TagStructure(Size = 0x8)]
            public class AnimationBlock : TagStructure
			{
                public int FrameNumber;
                public float Alpha;
            }
        }

        [TagStructure(Size = 0x20)]
        public class BitmapBlock : TagStructure
		{
            public TagBlock<AnimationBlock> Animation;
            public TagFunction Function = new TagFunction { Data = new byte[0] };

            [TagStructure(Size = 0x14)]
            public class AnimationBlock : TagStructure
			{
                public int FrameNumber;
                public RealPoint2d Movement1;
                public RealPoint2d Movement2;
            }
        }
    }
}