namespace TagTool.Tags.Resources
{
    [TagStructure(Name = "sound_resource_definition", Size = 0x14)]
    public class SoundResourceDefinition : TagStructure
    {
        [TagField(IsResourceData = true)]
        public TagData Data;
    }
}