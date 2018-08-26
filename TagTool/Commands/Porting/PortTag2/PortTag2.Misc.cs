using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagTool.Audio;
using TagTool.Cache;
using TagTool.Common;
using TagTool.Damage;
using TagTool.Geometry;
using TagTool.Havok;
using TagTool.Serialization;
using TagTool.Tags;
using TagTool.Tags.Definitions;

namespace TagTool.Commands.Porting
{
    public partial class PortTag2Command : Command
    {
        CachedTagInstance Handle_Tags_That_Are_Not_Ready_To_Be_Ported(CacheFile blam_cache, CacheFile.IndexItem blamTag)
        {
            var groupTag = blamTag.GroupTag;
            switch (groupTag.ToString())
            {
                case "shit": // use the global shit tag until shit tags are port-able
                    return CacheContext.GetTag<ShieldImpact>(@"globals\global_shield_impact_settings");

                case "sncl": // always use the default sncl tag
                    return CacheContext.GetTag<SoundClasses>(@"sound\sound_classes");

                case "rmw ": // Until water vertices port, always null water shaders to prevent the screen from turning blue. Can return 0x400F when fixed
                    return CacheContext.GetTag<ShaderWater>(@"levels\multi\riverworld\shaders\riverworld_water_rough");

                case "rmcs": // there are no rmcs tags in ms23, disable completely for now
                case "rmct": // Cortana shaders have no example in HO, they need a real port
                case "rmbk": // Unknown, black shaders don't exist in HO, only in ODST, might be just complete blackness
                    return CacheContext.GetTag<Shader>(@"shaders\invalid");

                case "rmhg" when Flags.HasFlag(PortingFlags.NoRmhg): // rmhg have register indexing issues currently
                    if (CacheContext.TryGetTag<ShaderHalogram>(blamTag.Name, out var rmhgInstance))
                        return rmhgInstance;
                    return CacheContext.GetTag<ShaderHalogram>(@"objects\ui\shaders\editor_gizmo");

                // Don't port rmdf tags when using ShaderTest (MatchShaders doesn't port either but that's handled elsewhere).
                case "rmdf" when Flags.HasFlag(PortingFlags.ShaderTest) && CacheContext.TagNames.ContainsValue(blamTag.Name) && blam_cache_version >= CacheVersion.Halo3Retail:
                    return CacheContext.GetTag<RenderMethodDefinition>(blamTag.Name);
                case "rmdf" when Flags.HasFlag(PortingFlags.ShaderTest) && !CacheContext.TagNames.ContainsValue(blamTag.Name) && blam_cache_version >= CacheVersion.Halo3Retail:
                    Console.WriteLine($"WARNING: Unable to locate `{blamTag.Name}.rmdf`; using `shaders\\shader.rmdf` instead.");
                    return CacheContext.GetTag<RenderMethodDefinition>(@"shaders\shader");
            }
            return null;
        }

        CachedTagInstance Handle_Shader_Tags_When_Not_Porting_Or_Matching_Shaders(CacheFile.IndexItem blamTag)
        {
            var groupTag = blamTag.GroupTag;
            if ((RenderMethodTagGroups.Contains(groupTag) || EffectTagGroups.Contains(groupTag)) &&
                (!Flags.HasFlag(PortingFlags.ShaderTest) && !Flags.HasFlag(PortingFlags.MatchShaders)))
            {
                switch (groupTag.ToString())
                {
                    case "beam":
                        return CacheContext.GetTag<BeamSystem>(@"objects\weapons\support_high\spartan_laser\fx\firing_3p");

                    case "cntl":
                        return CacheContext.GetTag<ContrailSystem>(@"objects\weapons\pistol\needler\fx\projectile");

                    case "decs":
                        return CacheContext.GetTag<DecalSystem>(@"fx\decals\impact_plasma\impact_plasma_medium\hard");

                    case "ltvl":
                        return CacheContext.GetTag<LightVolumeSystem>(@"objects\weapons\pistol\plasma_pistol\fx\charged\projectile");

                    case "prt3":
                        return CacheContext.GetTag<Particle>(@"fx\particles\energy\sparks\impact_spark_orange");

                    case "rmd ":
                        return CacheContext.GetTag<ShaderDecal>(@"objects\gear\human\military\shaders\human_military_decals");

                    case "rmfl":
                        return CacheContext.GetTag<ShaderFoliage>(@"levels\multi\riverworld\shaders\riverworld_tree_leafa");

                    case "rmtr":
                        return CacheContext.GetTag<ShaderTerrain>(@"levels\multi\riverworld\shaders\riverworld_ground");

                    case "rmrd":
                    case "rmsh":
                    case "rmss":
                        return CacheContext.GetTag<Shader>(@"shaders\invalid");
                }
            }
            return null;
        }

        // if true, return the edTag
        bool GetOrCreateTag(CacheFile.IndexItem blamTag, out TagGroup edGroup, out CachedTagInstance edTag)
        {
            edGroup = null;
            edTag = null;

            var groupTag = blamTag.GroupTag;
            if (TagGroup.Instances.ContainsKey(groupTag))
            {
                edGroup = TagGroup.Instances[groupTag];
            }
            else
            {
                edGroup = new TagGroup(
                    blamTag.GroupTag,
                    blamTag.ParentGroupTag,
                    blamTag.GrandparentGroupTag, CacheContext.GetStringId(blamTag.GroupName));
            }

            if ((groupTag == "snd!") && Flags.HasFlag(PortingFlags.NoAudio))
                return true;

            if (Flags.HasFlag(PortingFlags.NoElites) && groupTag == "bipd" && (blamTag.Name.Contains("elite") || blamTag.Name.Contains("dervish")))
                return true;

            // Find existing replaced tag
            if (ReplacedTags.ContainsKey(groupTag) && ReplacedTags[groupTag].Contains(blamTag.Name))
            {
                foreach (var entry in CacheContext.TagNames.Where(i => i.Value == blamTag.Name))
                {
                    var instance = CacheContext.GetTag(entry.Key);

                    if (instance.Group.Tag == groupTag)
                    {
                        edTag = instance;
                        return true;
                    }
                }
            }
            else if (!Flags.HasFlag(PortingFlags.New))
            {
                foreach (var instance in CacheContext.TagCache.Index.FindAllInGroup(groupTag))
                {
                    if (instance == null || !CacheContext.TagNames.ContainsKey(instance.Index))
                        continue;

                    if (CacheContext.TagNames[instance.Index] == blamTag.Name)
                    {
                        if (Flags.HasFlag(PortingFlags.Replace) && !DoNotReplaceGroups.Contains(instance.Group.Tag.ToString()))
                        {
                            if (!Flags.HasFlag(PortingFlags.Recursive))
                            {
                                Flags &= ~PortingFlags.Replace;
                                Flags |= PortingFlags.Recursive;
                            }

                            edTag = instance;
                            break;
                        }
                        else
                        {
                            edTag = instance;
                            return true;
                        }
                    }
                }
            }

            if (Flags.HasFlag(PortingFlags.New) && !Flags.HasFlag(PortingFlags.Recursive))
            {
                Flags &= ~PortingFlags.New;
                Flags |= PortingFlags.Recursive;
            }

            //
            // If isReplacing is true, check current tags if there is an existing instance to replace
            //

            var replacedTags = ReplacedTags.ContainsKey(groupTag) ?
                (ReplacedTags[groupTag] ?? new List<string>()) :
                new List<string>();

            replacedTags.Add(blamTag.Name);
            ReplacedTags[groupTag] = replacedTags;

            // Allocate Eldorado Tag
            if (edTag == null)
            {
                if (Flags.HasFlag(PortingFlags.UseNull))
                {
                    var i = CacheContext.TagCache.Index.ToList().FindIndex(n => n == null);

                    if (i >= 0)
                        CacheContext.TagCache.Index[i] = edTag = new CachedTagInstance(i, edGroup);
                }
                else
                {
                    edTag = CacheContext.TagCache.AllocateTag(edGroup);
                }
            }

            CacheContext.TagNames[edTag.Index] = blamTag.Name;

            return false;
        }

        void Preconversion(CacheFile blam_cache, object blamDefinition)
        {
            switch (blamDefinition)
            {
                case RenderModel mode when blam_cache_version < CacheVersion.Halo3Retail:
                    foreach (var material in mode.Materials)
                        material.RenderMethod = null;
                    break;

                case Scenario scenario when Flags.HasFlag(PortingFlags.NoSquads):
                    scenario.Squads = new List<Scenario.Squad>();
                    break;
                case Scenario scenario when Flags.HasFlag(PortingFlags.NoForgePalette):
                    scenario.SandboxEquipment.Clear();
                    scenario.SandboxGoalObjects.Clear();
                    scenario.SandboxScenery.Clear();
                    scenario.SandboxSpawning.Clear();
                    scenario.SandboxTeleporters.Clear();
                    scenario.SandboxVehicles.Clear();
                    scenario.SandboxWeapons.Clear();
                    break;

                    /*case ScenarioStructureBsp bsp:
                        foreach (var instance in bsp.InstancedGeometryInstances)
                            instance.Name = StringId.Invalid;
                        break;*/
            }
        }

        void Postconversion(CacheFile blam_cache, List<ConvertTagTask> tag_conversion_tasks, CacheFile.IndexItem blamTag, CachedTagInstance edTag, object blamDefinition, Stream cacheStream, Dictionary<ResourceLocation, Stream> resourceStreams)
        {
            switch (blamDefinition)
            {
                case AreaScreenEffect sefc:
                    if (blam_cache_version < CacheVersion.Halo3ODST)
                    {
                        sefc.GlobalHiddenFlags = AreaScreenEffect.HiddenFlagBits.UpdateThread | AreaScreenEffect.HiddenFlagBits.RenderThread;

                        foreach (var screenEffect in sefc.ScreenEffects)
                            screenEffect.HiddenFlags = AreaScreenEffect.HiddenFlagBits.UpdateThread | AreaScreenEffect.HiddenFlagBits.RenderThread;
                    }
                    if (blamTag.Name == @"levels\ui\mainmenu\sky\ui")
                    {
                        sefc.ScreenEffects[0].Unknown4 = 1E-19f;
                        sefc.ScreenEffects[0].Duration = 1E-19f;
                    }
                    break;

                case Bitmap bitm:
                    blamDefinition = ConvertBitmap(blam_cache, bitm, resourceStreams);
                    break;

                case CameraFxSettings cfxs:
                    blamDefinition = ConvertCameraFxSettings(cfxs);
                    break;

                case Character character:
                    blamDefinition = ConvertCharacter(blam_cache, character);
                    break;

                case ChudDefinition chdt:
                    blamDefinition = ConvertChudDefinition(chdt);
                    break;

                case ChudGlobalsDefinition chudGlobals:
                    blamDefinition = ConvertChudGlobalsDefinition(blam_cache, chudGlobals);
                    break;

                case Cinematic cine:
                    blamDefinition = ConvertCinematic(cine);
                    break;

                case CinematicScene cisc:
                    blamDefinition = ConvertCinematicScene(cisc);
                    break;

                case Dialogue udlg:
                    blamDefinition = ConvertDialogue(blam_cache, cacheStream, udlg);
                    break;

                case Effect effect when blam_cache_version == CacheVersion.Halo3Retail:
                    //foreach (var even in effect.Events)
                    //foreach (var particleSystem in even.ParticleSystems)
                    //particleSystem.Unknown7 = 1.0f / particleSystem.Unknown7;
                    break;

                case Globals matg:
                    blamDefinition = ConvertGlobals(blam_cache, matg, cacheStream);
                    break;

                case LensFlare lens:
                    blamDefinition = ConvertLensFlare(lens);
                    break;

                case ModelAnimationGraph jmad:
                    blamDefinition = ConvertModelAnimationGraph(blam_cache, cacheStream, resourceStreams, jmad);
                    break;

                case MultilingualUnicodeStringList unic:
                    blamDefinition = ConvertMultilingualUnicodeStringList(blam_cache, tag_conversion_tasks, cacheStream, resourceStreams, unic);
                    break;

                case Particle particle when blam_cache_version == CacheVersion.Halo3Retail:
                    // Shift all flags above 2 by 1.
                    particle.Flags = (particle.Flags & 0x3) + ((int)(particle.Flags & 0xFFFFFFFC) << 1);
                    break;

                // If there is no valid resource in the prtm tag, null the mode itself to prevent crashes
                case ParticleModel particleModel when blam_cache_version >= CacheVersion.Halo3Retail && particleModel.Geometry.Resource.Page.Index == -1:
                    blamDefinition = null;
                    break;

                case PhysicsModel phmo:
                    blamDefinition = ConvertPhysicsModel(blam_cache, phmo);
                    break;

                case Projectile proj:
                    blamDefinition = ConvertProjectile(proj);
                    break;

                case RasterizerGlobals rasg:
                    blamDefinition = ConvertRasterizerGlobals(rasg);
                    break;

                // If there is no valid resource in the mode tag, null the mode itself to prevent crashes (engineer head, harness)
                case RenderModel mode when blam_cache_version >= CacheVersion.Halo3Retail && mode.Geometry.Resource.Page.Index == -1:
                    blamDefinition = null;
                    break;

                case RenderModel renderModel when blam_cache_version < CacheVersion.Halo3Retail:
                    blamDefinition = ConvertGen2RenderModel(blam_cache, edTag, renderModel, resourceStreams);
                    break;

                case Scenario scnr:
                    blamDefinition = ConvertScenario(blam_cache, tag_conversion_tasks, cacheStream, resourceStreams, scnr, blamTag.Name);
                    break;

                case ScenarioLightmap sLdT:
                    blamDefinition = ConvertScenarioLightmap(blam_cache, tag_conversion_tasks, cacheStream, resourceStreams, blamTag.Name, sLdT);
                    break;

                case ScenarioLightmapBspData Lbsp:
                    blamDefinition = ConvertScenarioLightmapBspData(Lbsp);
                    break;

                case ScenarioStructureBsp sbsp:
                    blamDefinition = ConvertScenarioStructureBsp(blam_cache, sbsp, edTag, resourceStreams);
                    break;

                case SkyAtmParameters skya:
                    // Decrease secondary fog intensity (it's quite sickening in ms23)
                    // foreach (var atmosphere in skya.AtmosphereProperties)
                    // atmosphere.FogIntensity2 /= 36.0f;
                    break;

                case Sound sound:
                    blamDefinition = ConvertSound(blam_cache, tag_conversion_tasks, cacheStream, resourceStreams, sound);
                    break;

                case SoundLooping lsnd:
                    blamDefinition = ConvertSoundLooping(blam_cache, lsnd);
                    break;

                case SoundMix snmx:
                    blamDefinition = ConvertSoundMix(snmx);
                    break;

                case StructureDesign sddt:
                    blamDefinition = ConvertStructureDesign(sddt);
                    break;

                case Style style:
                    blamDefinition = ConvertStyle(blam_cache, style);
                    break;

                // Fix shotgun reloading
                case Weapon weapon when blamTag.Name == "objects\\weapons\\rifle\\shotgun\\shotgun":
                    weapon.Unknown24 = 1 << 16;
                    break;
                case Weapon weapon when blamTag.Name.EndsWith("\\weapon\\warthog_horn"):
                    foreach (var attach in weapon.Attachments)
                        attach.PrimaryScale = CacheContext.GetStringId("primary_rate_of_fire");
                    break;
            }
        }
    }
}