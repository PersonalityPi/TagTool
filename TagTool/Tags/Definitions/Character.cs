using TagTool.Ai;
using TagTool.Cache;
using System.Collections.Generic;

namespace TagTool.Tags.Definitions
{
    [TagStructure(MaxVersion = CacheVersion.Halo3Retail, Size = 0x1D4, Name = "character", Tag = "char")]
    [TagStructure(MinVersion = CacheVersion.Halo3ODST, Size = 0x1F8, Name = "character", Tag = "char")]
    public class Character : TagStructure
	{
        public uint Flags;
        public CachedTagInstance ParentCharacter;
        public CachedTagInstance Unit;
        /// <summary>
        /// Creature reference for swarm characters ONLY
        /// </summary>
        public CachedTagInstance Creature;
        public CachedTagInstance Style;
        public CachedTagInstance MajorCharacter;

        public TagBlock<CharacterVariant> Variants;
        public TagBlock<CharacterUnitDialogue> UnitDialogue;
        public TagBlock<CharacterGeneralProperties> GeneralProperties;
        public TagBlock<CharacterVitalityProperties> VitalityProperties;
        public TagBlock<CharacterPlacementProperties> PlacementProperties;
        public TagBlock<CharacterPerceptionProperties> PerceptionProperties;
        public TagBlock<CharacterLookProperties> LookProperties;
        public TagBlock<CharacterMovementProperties> MovementProperties;
        public TagBlock<CharacterFlockingProperties> FlockingProperties;
        public TagBlock<CharacterSwarmProperties> SwarmProperties;
        public TagBlock<CharacterReadyProperties> ReadyProperties;
        public TagBlock<CharacterEngageProperties> EngageProperties;
        public TagBlock<CharacterChargeProperties> ChargeProperties;
        public TagBlock<CharacterEvasionProperties> EvasionProperties;
        public TagBlock<CharacterCoverProperties> CoverProperties;
        public TagBlock<CharacterRetreatProperties> RetreatProperties;
        public TagBlock<CharacterSearchProperties> SearchProperties;
        public TagBlock<CharacterPreSearchProperties> PreSearchProperties;
        public TagBlock<CharacterIdleProperties> IdleProperties;
        public TagBlock<CharacterVocalizationProperties> VocalizationProperties;
        public TagBlock<CharacterBoardingProperties> BoardingProperties;

        [TagField(Padding = true, Length = 12, MaxVersion = CacheVersion.Halo3Retail)]
        public byte[] Unused1;
        
        public TagBlock<CharacterCombatformProperties> CombatformProperties;
       
        [TagField(Padding = true, Length = 24, MinVersion = CacheVersion.Halo3ODST)]
        public byte[] Unused2;

        [TagField(MinVersion = CacheVersion.Halo3ODST)]
        public TagBlock<CharacterEngineerProperties> EngineerProperties;

        [TagField(MinVersion = CacheVersion.Halo3ODST)]
        public TagBlock<CharacterInspectProperties> InspectProperties;

        public TagBlock<CharacterUnknownProperties> UnknownProperties;
        public TagBlock<CharacterWeaponsProperties> WeaponsProperties;
        public TagBlock<CharacterFiringPatternProperties> FiringPatternProperties;
        public TagBlock<CharacterGrenadesProperties> GrenadesProperties;
        public TagBlock<CharacterVehicleProperties> VehicleProperties;
        public TagBlock<CharacterMorphProperties> MorphProperties;
        public TagBlock<CharacterEquipmentProperties> EquipmentProperties;
        public TagBlock<CharacterMetagameProperties> MetagameProperties;
        public TagBlock<CharacterActAttachment> ActAttachments;
        
    }
}