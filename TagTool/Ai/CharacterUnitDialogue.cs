using TagTool.Tags;
using System.Collections.Generic;

namespace TagTool.Ai
{
    [TagStructure(Size = 0xC)]
    public class CharacterUnitDialogue : TagStructure
	{
        public TagBlock<CharacterDialogueVariation> DialogueVariations;
    }
}
