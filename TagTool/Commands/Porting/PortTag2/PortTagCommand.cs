using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
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
        private HaloOnlineCacheContext CacheContext { get; }
        private CacheFile BaseBlamCache;
        private CacheVersion blam_cache_version => BaseBlamCache.Version;

        private RenderGeometryConverter GeometryConverter { get; }

        private Dictionary<Tag, List<string>> ReplacedTags = new Dictionary<Tag, List<string>>();

        private List<Tag> RenderMethodTagGroups = new List<Tag> { new Tag("rmbk"), new Tag("rmcs"), new Tag("rmd "), new Tag("rmfl"), new Tag("rmhg"), new Tag("rmsh"), new Tag("rmss"), new Tag("rmtr"), new Tag("rmw "), new Tag("rmrd"), new Tag("rmct") };
        private List<Tag> EffectTagGroups = new List<Tag> { new Tag("beam"), new Tag("cntl"), new Tag("ltvl"), new Tag("decs"), new Tag("prt3") };

        private PortingFlags Flags { get; set; } = PortingFlags.Recursive | PortingFlags.Scripts | PortingFlags.MatchShaders;

        private static readonly string[] DoNotReplaceGroups = new[]
        {
            "glps",
            "glvs",
            "vtsh",
            "pixl",
            "rmdf",
            "rmt2"
        };

        public PortTag2Command(HaloOnlineCacheContext cacheContext, CacheFile blamCache) :
            base(true,

                "PortTag2",

                "Ports a tag from the current cache file. Options are:" + Environment.NewLine +
                "    Replace, Recursive, Single, New, UseNull, NoAudio, NoElites, NoForgePalette, NoSquads, Scripts, NoScripts, ShaderTest, MatchShaders, NoShaders" + Environment.NewLine + Environment.NewLine +

                "Replace: Replace tags of the same name when porting." + Environment.NewLine +
                "Recursive: Recursively port all tag references available." + Environment.NewLine +
                "Single: Port the specified tag instance using only existing tag references if available." + Environment.NewLine +
                "New: Create a new tag after the last index." + Environment.NewLine +
                "UseNull: Port a tag using nulled tag indices where available." + Environment.NewLine +
                "NoAudio: Ports everything except for audio tags unless existing tags are available." + Environment.NewLine +
                "NoElites: Ports everything except elite bipeds." + Environment.NewLine +
                "NoForgePalette: Clears the forge palette of any scenario tag when porting." + Environment.NewLine +
                "NoSquads: Clears the squads palette of any scenario tag when porting." + Environment.NewLine +
                "Scripts: Ports and adjusts scripts where possible." + Environment.NewLine +
                "NoScripts: Clears the scripts of any scenario tag when porting." + Environment.NewLine +
                "ShaderTest: TBD." + Environment.NewLine +
                "MatchShaders: Attempts to match any shader tags using existing render method tags when porting." + Environment.NewLine +
                "NoShaders: Uses default shader tags when porting." + Environment.NewLine +
                "Memory: Keeps cache in memory until the porting process is complete.",

                "PortTag2 [Options] <Tag>",

                "")
        {
            CacheContext = cacheContext;
            BaseBlamCache = blamCache;
            GeometryConverter = new RenderGeometryConverter(cacheContext, blamCache);
        }

        public override object Execute(List<string> args)
        {
            if (args.Count < 1)
                return false;

            var flagNames = Enum.GetNames(typeof(PortingFlags)).Select(name => name.ToLower());
            var flagValues = Enum.GetValues(typeof(PortingFlags)) as PortingFlags[];

            while (args.Count > 1)
            {
                var arg = args[0].ToLower();

                switch (arg)
                {
                    case "single":
                        Flags &= ~PortingFlags.Recursive;
                        break;

                    case "noscripts":
                        Flags &= ~PortingFlags.Scripts;
                        break;

                    case "noshaders":
                        Flags &= ~PortingFlags.MatchShaders;
                        break;

                    case "replace":
                        Flags |= PortingFlags.Replace;
                        break;

                    default:
                        {
                            if (!flagNames.Contains(arg))
                                throw new FormatException($"Invalid {typeof(PortingFlags).FullName}: {args[0]}");

                            for (var i = 0; i < flagNames.Count(); i++)
                                if (arg == flagNames.ElementAt(i))
                                    Flags |= flagValues[i];
                        }
                        break;
                }

                args.RemoveAt(0);
            }

            var initialStringIdCount = CacheContext.StringIdCache.Strings.Count;

            //
            // Convert Blam data to ElDorado data
            //

            var resourceStreams = new Dictionary<TagTool.Common.ResourceLocation, Stream>();

            using (var cacheStream = Flags.HasFlag(PortingFlags.Memory) ? new MemoryStream() : (Stream)CacheContext.OpenTagCacheReadWrite())
            {
                InitializeMemoryStreams(BaseBlamCache, cacheStream);

                if (Flags.HasFlag(PortingFlags.Memory))
                    using (var cacheFileStream = CacheContext.OpenTagCacheRead())
                        cacheFileStream.CopyTo(cacheStream);
                
                var legacy_tags = ParseLegacyTag(BaseBlamCache, args[0]);
                foreach (var blamTag in legacy_tags)
                {
                    var task = ConvertTagSync(/*cacheStream,*/ resourceStreams, blamTag);
                    try
                    {
                        //task.Wait();
                    }
                    catch (AggregateException ae)
                    {
                        //if (task.IsFaulted)
                        //{
                        //    Exception exception =
                        //        task.Exception.InnerExceptions != null && task.Exception.InnerExceptions.Count == 1
                        //            ? task.Exception.InnerExceptions[0]
                        //            : task.Exception;

                        //    ExceptionDispatchInfo.Capture(exception).Throw();
                        //}
                        //else
                        //{
                        //    throw ae.Flatten();
                        //}
                    }
                }

                if (Flags.HasFlag(PortingFlags.Memory))
                    using (var cacheFileStream = CacheContext.OpenTagCacheReadWrite())
                    {
                        cacheFileStream.Seek(0, SeekOrigin.Begin);
                        cacheFileStream.SetLength(cacheFileStream.Position);

                        cacheStream.Seek(0, SeekOrigin.Begin);
                        cacheStream.CopyTo(cacheFileStream);
                    }
            }

            if (initialStringIdCount != CacheContext.StringIdCache.Strings.Count)
                using (var stringIdCacheStream = CacheContext.OpenStringIdCacheReadWrite())
                    CacheContext.StringIdCache.Save(stringIdCacheStream);

            CacheContext.SaveTagNames();

            foreach (var entry in resourceStreams)
            {
                if (Flags.HasFlag(PortingFlags.Memory))
                    using (var resourceFileStream = CacheContext.OpenResourceCacheReadWrite(entry.Key))
                    {
                        resourceFileStream.Seek(0, SeekOrigin.Begin);
                        resourceFileStream.SetLength(resourceFileStream.Position);

                        entry.Value.Seek(0, SeekOrigin.Begin);
                        entry.Value.CopyTo(resourceFileStream);
                    }

                entry.Value.Close();
            }

            return true;
        }

        private List<CacheFile.IndexItem> ParseLegacyTag(CacheFile blam_cache, string tagSpecifier)
        {
            if (tagSpecifier.Length == 0 || (!char.IsLetter(tagSpecifier[0]) && !tagSpecifier.Contains('*')) || !tagSpecifier.Contains('.'))
                throw new Exception($"Invalid tag name: {tagSpecifier}");

            var tagIdentifiers = tagSpecifier.Split('.');

            if (!CacheContext.TryParseGroupTag(tagIdentifiers[1], out var groupTag))
                throw new Exception($"Invalid tag name: {tagSpecifier}");

            var tagName = tagIdentifiers[0];

            List<CacheFile.IndexItem> result = new List<CacheFile.IndexItem>();

            // find the CacheFile.IndexItem(s)
            if (tagName == "*") result = blam_cache.IndexItems.FindAll(
                item => item != null && groupTag == item.GroupTag);
            else result.Add(blam_cache.IndexItems.Find(
                item => item != null && groupTag == item.GroupTag && tagName == item.Name));

            if (result.Count == 0)
                throw new Exception($"Invalid tag name: {tagSpecifier}");

            return result;
        }

        MemoryStream[] read_memory_streams;
        CacheFile[] blam_caches;
        Mutex[] mutices;
        public void InitializeMemoryStreams(CacheFile blam_cache, Stream memory_stream)
        {
            // create a memory stream with the given number of threads
            var threads_count = Environment.ProcessorCount;
            read_memory_streams = new MemoryStream[threads_count];
            mutices = new Mutex[threads_count];
            blam_caches = new CacheFile[threads_count];
            for (var i = 0; i < threads_count; i++)
            {
                read_memory_streams[i] = new MemoryStream();
                {
                    memory_stream.Position = 0;
                    memory_stream.CopyTo(read_memory_streams[i]);
                    memory_stream.Position = 0;
                }
                read_memory_streams[i].Position = 0;
                HaloOnlineCacheContext base_ho_cache_context = blam_cache.CacheContext;
                switch (blam_cache)
                {
                    case CacheFileGen3 gen3:

                        var new_ho_cache_context = new HaloOnlineCacheContext(base_ho_cache_context.Directory);
                        blam_caches[i] = new CacheFileGen3(new_ho_cache_context, gen3.File, gen3.Version, true);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                mutices[i] = new Mutex();
            }
        }
        Dictionary

        public CachedTagInstance ConvertTagSync(/*Stream cacheStream, */Dictionary<TagTool.Common.ResourceLocation, Stream> resourceStreams, CacheFile.IndexItem blamTag)
        {
            CachedTagInstance result = null;

            var stream_index = Mutex.WaitAny(mutices);
            var mutex = mutices[stream_index];
            var cacheStream = read_memory_streams[stream_index];
            var blam_cache = blam_caches[stream_index];

            if(cacheStream.Position != 0)
            {
                throw new Exception("SHIT");
            }
            cacheStream.Position = 0;

            try
            {

                if (blamTag == null) return null;
                var groupTag = blamTag.GroupTag;

                // Handle tags that are not ready to be ported
                {
                    var unhandled_tag = Handle_Tags_That_Are_Not_Ready_To_Be_Ported(blam_cache, blamTag);
                    if (unhandled_tag != null) return unhandled_tag;
                }

                // Handle shader tags when not porting or matching shaders
                {
                    var shader_tag = Handle_Shader_Tags_When_Not_Porting_Or_Matching_Shaders(blamTag);
                    if (shader_tag != null) return shader_tag;
                }

                // Check to see if the ElDorado tag exists or create a new one
                bool return_edtag = GetOrCreateTag(blamTag, out TagGroup edGroup, out CachedTagInstance edTag);
                if (return_edtag) return edTag; // return existing tags etc.

                // Load the Blam tag definition
                var blamContext = new CacheSerializationContext(ref blam_cache, blamTag);
                var blamDefinition = blam_cache.Deserializer.Deserialize(blamContext, TagDefinition.Find(groupTag));

                // Perform pre-conversion fixups to the Blam tag definition
                Preconversion(blam_cache, blamDefinition);

                List<ConvertTagTask> tag_conversion_tasks = new List<ConvertTagTask>();

                // Perform automatic conversion on the Blam tag definition
                blamDefinition = ConvertData(blam_cache, tag_conversion_tasks, cacheStream, resourceStreams, blamDefinition, blamDefinition, blamTag.Name);

                // wait for all child tags to complete
                //await Task.WhenAll(tag_conversion_tasks.ToArray());
                Task.WaitAll(tag_conversion_tasks.ToArray());

                foreach(var task in tag_conversion_tasks)
                {
                    var assignment_operations = task.GetAssignmentOperations();
                    foreach(var assignment_operation in assignment_operations)
                    {
                        var task_result = task.Result;
                        assignment_operation.FieldInfo.SetValue(assignment_operation.Parent, result);
                    }
                }

                // Perform post-conversion fixups to Blam data
                Postconversion(blam_cache, tag_conversion_tasks, blamTag, edTag, blamDefinition, cacheStream, resourceStreams);

                //if (blamDefinition == null) //If blamDefinition is null, return null tag.
                //{
                //    CacheContext.TagNames.Remove(edTag.Index);
                //    CacheContext.TagCache.Index[edTag.Index] = null;
                //    return null;
                //}

                // Finalize and serialize the new ElDorado tag definition
                //var edContext = new TagSerializationContext(cacheStream, CacheContext, edTag);
                //CacheContext.Serializer.Serialize(edContext, blamDefinition);

                Console.WriteLine($"['{edTag.Group.Tag}', 0x{edTag.Index:X4}] {CacheContext.TagNames[edTag.Index]}.{CacheContext.GetString(edTag.Group.Name)}");

                result = edTag;
            }
            finally
            {
                cacheStream.Position = 0;
                mutex.ReleaseMutex();
            }
            return result;
        }

        private object ConvertData(
            CacheFile blam_cache,
            List<ConvertTagTask> tag_conversion_tasks,
            Stream cacheStream,
            Dictionary<TagTool.Common.ResourceLocation, Stream> resourceStreams,
            object data,
            object definition,
            string blamTagName,
            FieldInfo fieldinfo = null,
            object parent = null
            )
        {
            if (data == null)
                return null;

            var type = data.GetType();

            if (type.IsPrimitive)
                return data;

            switch (data)
            {
                case TagFunction tagFunction:
                    return ConvertTagFunction(tagFunction);

                case StringId stringId:
                    {
                        if (stringId == StringId.Invalid)
                            return stringId;

                        var value = blam_cache_version < CacheVersion.Halo3Retail ?
                            blam_cache.Strings.GetItemByID((int)(stringId.Value & 0xFFFF)) :
                            blam_cache.Strings.GetString(stringId);

                        var edStringId = blam_cache_version < CacheVersion.Halo3Retail ?
                            CacheContext.GetStringId(value) :
                            CacheContext.StringIdCache.GetStringId(stringId.Set, value);

                        if ((stringId != StringId.Invalid) && (edStringId != StringId.Invalid))
                            return edStringId;

                        if (((stringId != StringId.Invalid) && (edStringId == StringId.Invalid)) || !CacheContext.StringIdCache.Contains(value))
                            return CacheContext.StringIdCache.AddString(value);

                        return StringId.Invalid;
                    }

                case CachedTagInstance tag:
                    {
                        if (!Flags.HasFlag(PortingFlags.Recursive))
                        {
                            foreach (var instance in CacheContext.TagCache.Index.FindAllInGroup(tag.Group))
                            {
                                if (instance == null || !CacheContext.TagNames.ContainsKey(instance.Index))
                                    continue;

                                if (CacheContext.TagNames[instance.Index] == blamTagName)
                                    return instance;
                            }

                            return null;
                        }

                        tag = PortTagReference(blam_cache, tag.Index);

                        if (tag != null && !(Flags.HasFlag(PortingFlags.New) || Flags.HasFlag(PortingFlags.Replace)))
                            return tag;

                        if (fieldinfo != null && parent != null)
                        {
                            var blamTag = blam_cache.IndexItems.Find(i => i.ID == ((CachedTagInstance)data).Index);
                            var task = CreateConvertTagJob(cacheStream, resourceStreams, blamTag, parent, fieldinfo);
                            tag_conversion_tasks.Add(task);

                            // The job stores a field and parent value which will set this value when the job is completed
                            return null;
                        }
                        else
                        {
                            throw new Exception("Async tag conversion only, requires a parent/field for the job completion");
                            //HACK: Maybe, just maybe, we might introduce a synchronous option. But for now, nopety nope nope
                            //ConvertTag(cacheStream, resourceStreams, BlamCache.IndexItems.Find(i => i.ID == ((CachedTagInstance)data).Index));
                        }
                    }

                case CollisionMoppCode collisionMopp:
                    collisionMopp.Data = ConvertCollisionMoppData(blam_cache, collisionMopp.Data);
                    return collisionMopp;

                case DamageReportingType damageReportingType:
                    return ConvertDamageReportingType(blam_cache, damageReportingType);

                case GameObjectType gameObjectType:
                    return ConvertGameObjectType(blam_cache, gameObjectType);

                case ObjectTypeFlags objectTypeFlags:
                    return ConvertObjectTypeFlags(blam_cache, objectTypeFlags);

                case BipedPhysicsFlags bipedPhysicsFlags:
                    return ConvertBipedPhysicsFlags(blam_cache, bipedPhysicsFlags);

                case WeaponFlags weaponFlags:
                    return ConvertWeaponFlags(blam_cache, weaponFlags);

                case RenderMaterial.PropertyType propertyType when blam_cache_version < CacheVersion.Halo3Retail:
                    if (!Enum.TryParse(propertyType.Halo2.ToString(), out propertyType.Halo3))
                        throw new NotSupportedException(propertyType.Halo2.ToString());
                    break;

                case RenderMethod renderMethod when Flags.HasFlag(PortingFlags.MatchShaders):
                    ConvertData(blam_cache, tag_conversion_tasks, cacheStream, resourceStreams, renderMethod.ShaderProperties[0].ShaderMaps, renderMethod.ShaderProperties[0].ShaderMaps, blamTagName);
                    return ConvertRenderMethod(blam_cache, tag_conversion_tasks, cacheStream, resourceStreams, renderMethod, blamTagName);

                case ScenarioObjectType scenarioObjectType:
                    return ConvertScenarioObjectType(blam_cache, scenarioObjectType);

                case SoundClass soundClass:
                    return soundClass.ConvertSoundClass(blam_cache_version);
            }

            if (type.IsArray)
                return ConvertArray(blam_cache, tag_conversion_tasks, cacheStream, resourceStreams, (Array)data, definition, blamTagName);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                return ConvertList(blam_cache, tag_conversion_tasks, cacheStream, resourceStreams, data, type, definition, blamTagName);

            if (type.GetCustomAttributes(typeof(TagStructureAttribute), false).Length > 0)
            {
                data = ConvertStructure(blam_cache, tag_conversion_tasks, cacheStream, resourceStreams, data, type, definition, blamTagName);

                switch (data)
                {
                    case RenderGeometry renderGeometry when blam_cache_version >= CacheVersion.Halo3Retail:
                        return GeometryConverter.Convert(cacheStream, renderGeometry, resourceStreams, Flags);

                    case Mesh.Part part when blam_cache_version < CacheVersion.Halo3Retail:
                        if (!Enum.TryParse(part.TypeOld.ToString(), out part.TypeNew))
                            throw new NotSupportedException(part.TypeOld.ToString());
                        break;

                    case RenderMaterial.Property property when blam_cache_version < CacheVersion.Halo3Retail:
                        property.IntValue = property.ShortValue;
                        break;

                    case Model.GlobalDamageInfoBlock newDamageInfo:
                        return ConvertNewDamageInfo(newDamageInfo);
                }
            }

            return data;
        }

        private Array ConvertArray(CacheFile blam_cache, List<ConvertTagTask> tag_conversion_tasks, Stream cacheStream, Dictionary<TagTool.Common.ResourceLocation, Stream> resourceStreams, Array array, object definition, string blamTagName)
        {
            if (array.GetType().GetElementType().IsPrimitive)
                return array;

            for (var i = 0; i < array.Length; i++)
            {
                var oldValue = array.GetValue(i);
                var newValue = ConvertData(blam_cache, tag_conversion_tasks, cacheStream, resourceStreams, oldValue, definition, blamTagName);
                array.SetValue(newValue, i);
            }

            return array;
        }

        private object ConvertList(CacheFile blam_cache, List<ConvertTagTask> tag_conversion_tasks, Stream cacheStream, Dictionary<TagTool.Common.ResourceLocation, Stream> resourceStreams, object list, Type type, object definition, string blamTagName)
        {
            if (type.GenericTypeArguments[0].IsPrimitive)
                return list;

            var count = (int)type.GetProperty("Count").GetValue(list);

            var getItem = type.GetMethod("get_Item");
            var setItem = type.GetMethod("set_Item");

            for (var i = 0; i < count; i++)
            {
                var oldValue = getItem.Invoke(list, new object[] { i });
                var newValue = ConvertData(blam_cache, tag_conversion_tasks, cacheStream, resourceStreams, oldValue, definition, blamTagName);
                setItem.Invoke(list, new object[] { i, newValue });
            }

            return list;
        }

        private object ConvertStructure(CacheFile blam_cache, List<ConvertTagTask> tag_conversion_tasks, Stream cacheStream, Dictionary<TagTool.Common.ResourceLocation, Stream> resourceStreams, object data, Type type, object definition, string blamTagName)
        {
            using (var enumerator = ReflectionCache.GetTagFieldEnumerator(type, CacheContext.Version))
            {
                while (enumerator.Next())
                {
                    var oldValue = enumerator.Field.GetValue(data);
                    var newValue = ConvertData(blam_cache, tag_conversion_tasks, cacheStream, resourceStreams, oldValue, definition, blamTagName, enumerator.Field, data);
                    enumerator.Field.SetValue(data, newValue);
                }
            }
            return data;
        }

        private Model.GlobalDamageInfoBlock ConvertNewDamageInfo(Model.GlobalDamageInfoBlock newDamageInfo)
        {
            if (!Enum.TryParse(newDamageInfo.CollisionDamageReportingTypeOld.HaloOnline.ToString(), out newDamageInfo.CollisionDamageReportingTypeNew))
                newDamageInfo.CollisionDamageReportingTypeNew = Model.GlobalDamageInfoBlock.DamageReportingTypeNew.Guardians;

            if (!Enum.TryParse(newDamageInfo.ResponseDamageReportingTypeOld.HaloOnline.ToString(), out newDamageInfo.ResponseDamageReportingTypeNew))
                newDamageInfo.ResponseDamageReportingTypeNew = Model.GlobalDamageInfoBlock.DamageReportingTypeNew.Guardians;

            return newDamageInfo;
        }

        private ObjectTypeFlags ConvertObjectTypeFlags(CacheFile blam_cache, ObjectTypeFlags objectTypeFlags)
        {
            if (blam_cache_version == CacheVersion.Halo3Retail)
            {
                if (!Enum.TryParse(objectTypeFlags.Halo3Retail.ToString(), out objectTypeFlags.Halo3ODST))
                    throw new FormatException(blam_cache_version.ToString());
            }

            return objectTypeFlags;
        }

        private BipedPhysicsFlags ConvertBipedPhysicsFlags(CacheFile blam_cache, BipedPhysicsFlags bipedPhysicsFlags)
        {
            if (!Enum.TryParse(bipedPhysicsFlags.Halo3Retail.ToString(), out bipedPhysicsFlags.Halo3Odst))
                throw new FormatException(blam_cache_version.ToString());

            return bipedPhysicsFlags;
        }

        private object ConvertWeaponFlags(CacheFile blam_cache, WeaponFlags weaponFlags)
        {
            if (weaponFlags.OldFlags.HasFlag(WeaponFlags.OldWeaponFlags.WeaponUsesOldDualFireErrorCode))
                weaponFlags.OldFlags &= ~WeaponFlags.OldWeaponFlags.WeaponUsesOldDualFireErrorCode;

            if (!Enum.TryParse(weaponFlags.OldFlags.ToString(), out weaponFlags.NewFlags))
                throw new FormatException(blam_cache_version.ToString());

            return weaponFlags;
        }

        private DamageReportingType ConvertDamageReportingType(CacheFile blam_cache, DamageReportingType damageReportingType)
        {
            string value = null;

            switch (blam_cache_version)
            {
                case CacheVersion.Halo2Vista:
                case CacheVersion.Halo2Xbox:
                    value = damageReportingType.Halo2Retail.ToString();
                    break;

                case CacheVersion.Halo3ODST:
                    value = damageReportingType.Halo3ODST.ToString();
                    break;

                case CacheVersion.Halo3Retail:
                    value = damageReportingType.Halo3Retail.ToString();
                    break;
            }

            if (value == null || !Enum.TryParse(value, out damageReportingType.HaloOnline))
                throw new NotSupportedException(value ?? CacheContext.Version.ToString());

            return damageReportingType;
        }

        private TagFunction ConvertTagFunction(TagFunction function)
        {
            return TagFunction.ConvertTagFunction(function);
        }

        private GameObjectType ConvertGameObjectType(CacheFile blam_cache, GameObjectType objectType)
        {
            if (blam_cache_version == CacheVersion.Halo3Retail)
                if (!Enum.TryParse(objectType.Halo3Retail.ToString(), out objectType.Halo3ODST))
                    throw new FormatException(blam_cache_version.ToString());

            return objectType;
        }

        private ScenarioObjectType ConvertScenarioObjectType(CacheFile blam_cache, ScenarioObjectType objectType)
        {
            if (blam_cache_version == CacheVersion.Halo3Retail)
                if (!Enum.TryParse(objectType.Halo3Retail.ToString(), out objectType.Halo3ODST))
                    throw new FormatException(blam_cache_version.ToString());

            return objectType;
        }

        private CachedTagInstance PortTagReference(CacheFile blam_cache, int index, int maxIndex = 0xFFFF)
        {
            if (index == -1)
                return null;

            var instance = blam_cache.IndexItems.Find(i => i.ID == index);

            if (instance != null)
            {
                var tags = CacheContext.TagCache.Index.FindAllInGroup(instance.GroupTag);

                try
                {
                    foreach (var tag in tags)
                    {
                        if (!CacheContext.TagNames.ContainsKey(tag.Index))
                            continue;

                        if (instance.Name == CacheContext.TagNames[tag.Index] && tag.Index < maxIndex)
                            return tag;
                    }
                }
                catch
                {
                    Console.WriteLine();
                }

            }

            return null;
        }
    }
}