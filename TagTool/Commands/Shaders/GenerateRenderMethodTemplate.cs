using TagTool.Cache;
using TagTool.Commands;
using TagTool.Geometry;
using TagTool.Serialization;
using TagTool.Shaders;
using TagTool.Tags.Definitions;
using System;
using System.Collections.Generic;
using System.IO;
using TagTool.Common;
using TagTool.ShaderGenerator;
using TagTool.ShaderGenerator.Types;
using System.Linq;
using static TagTool.Tags.Definitions.RenderMethodTemplate.DrawModeRegisterOffsetBlock;
using TagTool.ShaderGenerator.RegisterFixups;

namespace TagTool.Commands.Shaders
{
    class GenerateRenderMethodTemplate : Command
    {
        private GameCacheContext CacheContext { get; }
        private CachedTagInstance rmt2_cachedtaginstance { get; }
        private RenderMethodTemplate rmt2 { get; }

        public GenerateRenderMethodTemplate(GameCacheContext cacheContext, CachedTagInstance tag, RenderMethodTemplate definition) :
            base(CommandFlags.Inherit,

                "Generate",
                "Compiles HLSL source file from scratch :D",
                "Generate <shader_type> <parameters...>",
                "Compiles HLSL source file from scratch :D")
        {
            CacheContext = cacheContext;
            rmt2_cachedtaginstance = tag;
            rmt2 = definition;
        }

        private void CreateShader<T>(RenderMethodTemplate.ShaderMode mode, Dictionary<RenderMethodTemplate.ShaderMode, ShaderGeneratorResult> shader_results, int[] shader_args) where T : class
        {
            dynamic generator = Activator.CreateInstance(typeof(T), new object[] { CacheContext, mode, shader_args, 0 });
            shader_results[mode] = generator?.Generate();
        }

        class MaterialFixupInformation
        {
            public Dictionary<string, RenderMethod.ShaderProperty.ShaderMap> ShaderMaps = new Dictionary<string, RenderMethod.ShaderProperty.ShaderMap>();
            public Dictionary<string, RenderMethod.ShaderProperty.Argument> Arguments = new Dictionary<string, RenderMethod.ShaderProperty.Argument>();

        }

        public override object Execute(List<string> args)
        {
            if (args.Count <= 0)
                return false;

            if(args.Count < 2)
            {
                Console.WriteLine("Invalid number of args");
                return false;
            }

            string shader_type;
            try
            {
                shader_type = args[0].ToLower();
            } catch
            {
                Console.WriteLine("Invalid index, type, and drawmode combination");
                return false;
            }

            Int32[] shader_args;
			try { shader_args = Array.ConvertAll(args.Skip(1).ToArray(), Int32.Parse); }
			catch { Console.WriteLine("Invalid shader arguments! (could not parse to Int32[].)"); return false; }

            var stream = CacheContext.TagCacheFile.Open(FileMode.Open, FileAccess.ReadWrite);

            // Shader Dependencies
            var shaders_dependencies_instances = CacheContext.TagCache.Index.NonNull().Where(t => t.Dependencies.Contains(rmt2_cachedtaginstance.Index)).ToList();
            List<RenderMethod> shaders_dependencies = new List<RenderMethod>();

            foreach (var shader_dependency_instance in shaders_dependencies_instances)
            {
                //using (var stream = CacheContext.OpenTagCacheRead())
                {
                    var shader_dependency = CacheContext.Deserializer.Deserialize<RenderMethod>(new TagSerializationContext(stream, CacheContext, shader_dependency_instance));
                    shaders_dependencies.Add(shader_dependency);
                }
            }

            Dictionary<RenderMethod, MaterialFixupInformation> shader_fixups = new Dictionary<RenderMethod, MaterialFixupInformation>();
            foreach(var shader in shaders_dependencies)
            {
                if (shader.ShaderProperties.Count > 1) throw new NotImplementedException();

                var properties = shader.ShaderProperties[0];

                if (properties.Template.Index != rmt2_cachedtaginstance.Index) throw new Exception("What the mathematical???");

                MaterialFixupInformation fixupInformation = new MaterialFixupInformation();
                for(int i=0;i<properties.ShaderMaps.Count;i++)
                {
                    var shader_reference = CacheContext.GetString(rmt2.ShaderMaps[i].Name);
                    fixupInformation.ShaderMaps[shader_reference] = properties.ShaderMaps[i];
                }

                Dictionary<string, RenderMethod.ShaderProperty.Argument> Arguments = new Dictionary<string, RenderMethod.ShaderProperty.Argument>();
                for (int i = 0; i < properties.Arguments.Count; i++)
                {
                    var argument_name = CacheContext.GetString(rmt2.Arguments[i].Name);
                    fixupInformation.Arguments[argument_name] = properties.Arguments[i];
                }

                shader_fixups[shader] = fixupInformation;
            }

            Dictionary<RenderMethodTemplate.ShaderMode, ShaderGeneratorResult> shader_results = new Dictionary<RenderMethodTemplate.ShaderMode, ShaderGeneratorResult>();

            // Reset all information
            rmt2.DrawModeBitmask = 0;
            rmt2.DrawModes = new List<RenderMethodTemplate.RMT2PackedUInt16>();
            rmt2.ArgumentMappings = new List<RenderMethodTemplate.ArgumentMapping>();
            rmt2.DrawModeRegisterOffsets = new List<RenderMethodTemplate.DrawModeRegisterOffsetBlock>();
            rmt2.Arguments = new List<RenderMethodTemplate.ShaderArgument>();
            rmt2.Unknown5 = new List<RenderMethodTemplate.ShaderArgument>();
            rmt2.GlobalArguments = new List<RenderMethodTemplate.ShaderArgument>();
            rmt2.ShaderMaps = new List<RenderMethodTemplate.ShaderArgument>();

            // runs the appropriate shader-generator for the template type.
            switch (shader_type)
            {
                case "beam_templates":
                case "beam_template":
                    CreateShader<BeamTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Default, shader_results, shader_args);
					break;
                case "contrail_templates":
                case "contrail_template":
                    CreateShader<ContrailTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Default, shader_results, shader_args);
                    break;
                case "decal_templates":
                case "decal_template":
                    CreateShader<DecalTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Default, shader_results, shader_args);
                    break;
                case "shader_templates":
                case "shader_template":
                    CreateShader<ShaderTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Albedo, shader_results, shader_args);
                    CreateShader<ShaderTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Static_Per_Pixel, shader_results, shader_args);
                    //CreateShader<ShaderTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Static_Per_Vertex, shader_results, shader_args);
                    //CreateShader<ShaderTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Static_Sh, shader_results, shader_args);
                    //CreateShader<ShaderTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Static_Prt_Ambient, shader_results, shader_args);
                    //CreateShader<ShaderTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Static_Prt_Linear, shader_results, shader_args);
                    //CreateShader<ShaderTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Static_Prt_Quadratic, shader_results, shader_args);
                    //CreateShader<ShaderTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Dynamic_Light, shader_results, shader_args);
                    //CreateShader<ShaderTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Shadow_Generate, shader_results, shader_args);
                    //CreateShader<ShaderTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Active_Camo, shader_results, shader_args);
                    //CreateShader<ShaderTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Shadow_Generate, shader_results, shader_args);
                    //CreateShader<ShaderTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Lightmap_Debug_Mode, shader_results, shader_args);
                    //CreateShader<ShaderTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Static_Per_Vertex_Color, shader_results, shader_args);
                    //CreateShader<ShaderTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Dynamic_Light_Cinematic, shader_results, shader_args);
                    //CreateShader<ShaderTemplateShaderGenerator>(RenderMethodTemplate.ShaderMode.Sfx_Distort, shader_results, shader_args);
                    break;
				case "cortana_templates":
				case "cortana_template":
				case "custom_templates":
				case "custom_template":
                case "foliage_templates":
                case "foliage_template":
                case "halogram_templates":
                case "halogram_template":
                case "light_volume_templates":
                case "light_volume_template":
				case "particle_templates":
				case "particle_template":
				case "screen_templates":
				case "screen_template":
                case "terrain_templates":
                case "terrain_template":
                case "water_templates":
                case "water_template":
                    Console.WriteLine($"{shader_type} is not implemented");
                    return false;
                default:
                    Console.WriteLine($"Unknown template {shader_type}");
                    return false;
            }

            List<RenderMethodTemplate.ShaderMode> shader_modes = new List<RenderMethodTemplate.ShaderMode>(shader_results.Keys);

            PixelShader pixl;
            CachedTagInstance pixl_cachedtaginstance;

            //using (var stream = CacheContext.OpenTagCacheRead())
            {
                pixl_cachedtaginstance = rmt2.PixelShader;
                pixl = (PixelShader)CacheContext.Deserializer.Deserialize(new TagSerializationContext(stream, CacheContext, pixl_cachedtaginstance), typeof(PixelShader));
            }

            int num_drawmodes = 0;
            foreach (var result in shader_results)
            {
                num_drawmodes = Math.Max(num_drawmodes, (int)result.Key + 1);
            }
            // Reset information and allocate sapce
            rmt2.DrawModes = new RenderMethodTemplate.RMT2PackedUInt16[num_drawmodes].ToList();
            pixl.DrawModes = new ShaderDrawMode[num_drawmodes].ToList();
            pixl.Shaders = new List<PixelShaderBlock>();

            int registers_total = 0;
            int registers_success = 0;
            int registers_failure = 0;

            foreach (var result in shader_results)
            {
                var shader_mode = result.Key;
                // We need to add in the RMT2 Drawmode Register Offsets for this

                var result_default = result.Value;
                var shader_mode_bitmask = (RenderMethodTemplate.ShaderModeBitmask)(1 << (int)shader_mode);

                //TODO: When we support global pixel shaders, remove this check
                if(result_default != null)
                {
                    rmt2.DrawModeBitmask |= shader_mode_bitmask;
                }
                
                if(result_default != null)
                {
                    Register_Fixup.Register_Fixup_Manager manager = new Register_Fixup.Register_Fixup_Manager(CacheContext, result_default.Parameters, null);

                    Register_Fixups_Textures_Samplers register_Fixups_Textures_Samplers = new Register_Fixups_Textures_Samplers();
                    register_Fixups_Textures_Samplers.Fixup(manager);

                    Register_Fixups_Arguments register_Fixups_Arguments = new Register_Fixups_Arguments();
                    register_Fixups_Arguments.Fixup(manager);

                    Register_Fixups_Render_Method_Extern_Vectors register_Fixups_Render_Method_Extern_Vectors = new Register_Fixups_Render_Method_Extern_Vectors();
                    register_Fixups_Render_Method_Extern_Vectors.Fixup(manager);

                    foreach (var target in manager.Targets)
                    {
                        registers_total++;
                        if (target.IsHandled)
                        {
                            registers_success++;
                            Console.WriteLine($"SUCCESS: {target.Name}");
                        }
                        else
                        {
                            registers_failure++;
                            Console.WriteLine($"FAILURE: {target.Name}");
                        }
                    }

                    foreach (var texture_sampler in manager.StringIDs_Textures_Samplers)
                    {
                        rmt2.ShaderMaps.Add(new RenderMethodTemplate.ShaderArgument
                        {
                            Name = CacheContext.GetStringId(texture_sampler)
                        });
                    }

                    //TODO:
                    var drawmode = new RenderMethodTemplate.RMT2PackedUInt16();
                    drawmode.Offset = (ushort)rmt2.DrawModeRegisterOffsets.Count();
                    drawmode.Count = 1;
                    rmt2.DrawModes[(int)shader_mode] = drawmode;

                    //TODO:
                    var offset_block = new RenderMethodTemplate.DrawModeRegisterOffsetBlock();

                    offset_block.Textures_Samplers = new RenderMethodTemplate.RMT2PackedUInt16();
                    offset_block.Textures_Samplers.Offset = (ushort)rmt2.ArgumentMappings.Count();
                    offset_block.Textures_Samplers.Count = (ushort)manager.Targets_Textures_Samplers.Where(target => target.IsHandledDirectly).Count();

                    foreach (var textures_sampler_argument in manager.Targets_Textures_Samplers)
                    {
                        var shader_argument = new RenderMethodTemplate.ArgumentMapping();

                        // No need to handle indirect registers
                        if (textures_sampler_argument.IsHandledDirectly)
                        {
                            shader_argument.RegisterIndex = textures_sampler_argument.Parameter.RegisterIndex;
                            shader_argument.ArgumentIndex = (byte)textures_sampler_argument.ArgumentIndex;

                            //TODO: Unknown argument

                            rmt2.ArgumentMappings.Add(shader_argument);
                        }
                    }

                    offset_block.Arguments_Vectors = new RenderMethodTemplate.RMT2PackedUInt16();
                    offset_block.Arguments_Vectors.Offset = (ushort)rmt2.ArgumentMappings.Count();
                    offset_block.Arguments_Vectors.Count = (ushort)manager.Targets_Arguments.Where(target => target.IsHandledDirectly).Count();

                    // Add all of the string ids
                    foreach (var argument in manager.StringIDs_Arguments)
                    {
                        var argument_index = -1;
                        foreach (var current_argument in rmt2.Arguments)
                        {
                            var arg_str = CacheContext.GetString(current_argument.Name);
                            if (arg_str == argument)
                            {
                                argument_index = rmt2.Arguments.IndexOf(current_argument);
                                break;
                            }
                        }
                        if (argument_index == -1)
                        {
                            argument_index = rmt2.Arguments.Count();
                            rmt2.Arguments.Add(new RenderMethodTemplate.ShaderArgument
                            {
                                //TODO: Implement these xform's as handled arguments and add to the list for associated textures
                                Name = CacheContext.GetStringId(argument.Replace("_xform", ""))
                            });
                        }
                    }

                    foreach (var arguments_vectors_argument in manager.Targets_Arguments)
                    {
                        var shader_argument = new RenderMethodTemplate.ArgumentMapping();

                        // No need to handle indirect registers
                        if (arguments_vectors_argument.IsHandledDirectly)
                        {
                            shader_argument.RegisterIndex = arguments_vectors_argument.Parameter.RegisterIndex;

                            var argument_index = -1;
                            foreach(var argument in rmt2.Arguments)
                            {
                                if(CacheContext.GetString(argument.Name) == arguments_vectors_argument.Name.Replace("_xform", ""))
                                {
                                    argument_index = rmt2.Arguments.IndexOf(argument);
                                }
                            }
                            if (argument_index == -1) throw new Exception("This shouldn't happen");

                            shader_argument.ArgumentIndex = (byte)argument_index;

                            //TODO: Unknown argument

                            rmt2.ArgumentMappings.Add(shader_argument);
                        }
                    }

                    offset_block.Render_Method_Extern_Vectors = new RenderMethodTemplate.RMT2PackedUInt16();
                    offset_block.Render_Method_Extern_Vectors.Offset = (ushort)rmt2.ArgumentMappings.Count();
                    offset_block.Render_Method_Extern_Vectors.Count = (ushort)manager.Targets_Render_Method_Extern_Vectors_Targets.Where(target => target.IsHandledDirectly).Count();

                    foreach (var render_method_extern_vector_argument in manager.Targets_Render_Method_Extern_Vectors_Targets)
                    {
                        var shader_argument = new RenderMethodTemplate.ArgumentMapping();

                        // No need to handle indirect registers
                        if (render_method_extern_vector_argument.IsHandledDirectly)
                        {
                            shader_argument.RegisterIndex = render_method_extern_vector_argument.Parameter.RegisterIndex;
                            shader_argument.ArgumentIndex = (byte)render_method_extern_vector_argument.ArgumentIndex;

                            //TODO: Unknown argument

                            rmt2.ArgumentMappings.Add(shader_argument);
                        }
                    }

                    rmt2.DrawModeRegisterOffsets.Add(offset_block);

                    var pixl_drawmode = new ShaderDrawMode();
                    pixl_drawmode.Index = (byte)pixl.Shaders.Count();
                    pixl_drawmode.Count = 1;

                    var pixl_shaderblock = new PixelShaderBlock();
                    pixl_shaderblock.PCParameters = result_default.Parameters;
                    pixl_shaderblock.PCShaderBytecode = result_default.ByteCode;

                    pixl.Shaders.Add(pixl_shaderblock);
                    pixl.DrawModes[(int)shader_mode] = pixl_drawmode;
                }
                else
                {
                    // This is a global shader, we need to specially account for this

                    var drawmode = new RenderMethodTemplate.RMT2PackedUInt16();
                    drawmode.Offset = (ushort)rmt2.DrawModeRegisterOffsets.Count();
                    drawmode.Count = 1;
                    rmt2.DrawModes[(int)shader_mode] = drawmode;

                    var offset_block = new RenderMethodTemplate.DrawModeRegisterOffsetBlock();

                    //TODO: Find global pixel shader and setup arguments

                    rmt2.DrawModeRegisterOffsets.Add(offset_block);
                    
                    var pixl_drawmode = new ShaderDrawMode();
                    pixl_drawmode.Index = 0;
                    pixl_drawmode.Count = 0;
                    pixl.DrawModes[(int)shader_mode] = pixl_drawmode;
                }
            }

            Console.WriteLine($"Register Fixups >> total:{registers_total} success:{registers_success} failure:{registers_failure}");

            //using (var stream = CacheContext.TagCacheFile.Open(FileMode.Open, FileAccess.ReadWrite))
            {
                var context = new TagSerializationContext(stream, CacheContext, rmt2_cachedtaginstance);
                CacheContext.Serializer.Serialize(context, rmt2);
            }

            //using (var stream = CacheContext.TagCacheFile.Open(FileMode.Open, FileAccess.ReadWrite))
            {
                var context = new TagSerializationContext(stream, CacheContext, pixl_cachedtaginstance);
                CacheContext.Serializer.Serialize(context, pixl);
            }

            for(var i=0;i<shaders_dependencies.Count;i++)
            {
                var shader_tag_cache = shaders_dependencies_instances[i];
                var shader = shaders_dependencies[i];
                var fixupInformation = shader_fixups[shader];

                if (shader.ShaderProperties.Count > 1) throw new NotImplementedException();
                var properties = shader.ShaderProperties[0];
                if (properties.Template.Index != rmt2_cachedtaginstance.Index) throw new Exception("What the mathematical???");

                properties.ShaderMaps = new List<RenderMethod.ShaderProperty.ShaderMap>();
                properties.Arguments = new List<RenderMethod.ShaderProperty.Argument>();
                properties.Functions = new List<RenderMethod.ShaderProperty.FunctionBlock>();

                foreach (var property in rmt2.ShaderMaps)
                {
                    var shader_reference = CacheContext.GetString(property.Name);
                    properties.ShaderMaps.Add(fixupInformation.ShaderMaps[shader_reference]);
                }

                foreach (var property in rmt2.Arguments)
                {
                    var shader_reference = CacheContext.GetString(property.Name);
                    properties.Arguments.Add(fixupInformation.Arguments[shader_reference]);
                }

                // Save
                //using (var stream = CacheContext.TagCacheFile.Open(FileMode.Open, FileAccess.ReadWrite))
                {
                    var context = new TagSerializationContext(stream, CacheContext, shader_tag_cache);
                    CacheContext.Serializer.Serialize(context, shader);
                }
            }

            stream.Close();

            return true;
        }

        public List<ShaderParameter> GetParamInfo(string assembly)
        {
            var parameters = new List<ShaderParameter> { };

            using (StringReader reader = new StringReader(assembly))
            {
                if (string.IsNullOrEmpty(assembly))
                    return null;

                string line;

                while (!(line = reader.ReadLine()).Contains("//   -"))
                    continue;

                while (!string.IsNullOrEmpty((line = reader.ReadLine())))
                {
                    line = (line.Replace("//   ", "").Replace("//", "").Replace(";", ""));

                    while (line.Contains("  "))
                        line = line.Replace("  ", " ");

                    if (!string.IsNullOrEmpty(line))
                    {
                        var split = line.Split(' ');
                        parameters.Add(new ShaderParameter
                        {
                            ParameterName = CacheContext.GetStringId(split[0]),
                            RegisterType = (ShaderParameter.RType)Enum.Parse(typeof(ShaderParameter.RType), split[1][0].ToString()),
                            RegisterIndex = byte.Parse(split[1].Substring(1)),
                            RegisterCount = byte.Parse(split[2])
                        });
                    }
                }
            }

            return parameters;
        }
    }
}