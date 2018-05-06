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

            // Reset all information
            rmt2.DrawModeBitmask = 0;
            rmt2.DrawModes = new List<RenderMethodTemplate.RMT2PackedUInt16>();
            rmt2.ArgumentMappings = new List<RenderMethodTemplate.ArgumentMapping>();
            rmt2.DrawModeRegisterOffsets = new List<RenderMethodTemplate.DrawModeRegisterOffsetBlock>();
            rmt2.Arguments = new List<RenderMethodTemplate.ShaderArgument>();
            rmt2.Unknown5 = new List<RenderMethodTemplate.ShaderArgument>();
            rmt2.GlobalArguments = new List<RenderMethodTemplate.ShaderArgument>();
            rmt2.ShaderMaps = new List<RenderMethodTemplate.ShaderArgument>();

            Dictionary<RenderMethodTemplate.ShaderMode, ShaderGeneratorResult> shader_results = new Dictionary<RenderMethodTemplate.ShaderMode, ShaderGeneratorResult>();

            // runs the appropriate shader-generator for the template type.
            switch (shader_type)
            {
                case "beam_templates":
                case "beam_template":
                    {
                        var result_default = new BeamTemplateShaderGenerator(CacheContext, RenderMethodTemplate.ShaderMode.Default, shader_args)?.Generate();
                        shader_results[RenderMethodTemplate.ShaderMode.Default] = result_default;
                    }
					break;
                case "shader_templates":
                case "shader_template":
                    {
                        var result_albedo = new ShaderTemplateShaderGenerator(CacheContext, RenderMethodTemplate.ShaderMode.Albedo, shader_args)?.Generate();
                        shader_results[RenderMethodTemplate.ShaderMode.Albedo] = result_albedo;
                    }
                    break;
                case "contrail_templates":
                case "contrail_template":
				case "cortana_templates":
				case "cortana_template":
				case "custom_templates":
				case "custom_template":
				case "decal_templates":
                case "decal_template":
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

            int num_drawmodes = 0;
            foreach (var result in shader_results)
            {
                num_drawmodes = Math.Max(Math.Max(1, num_drawmodes), (int)result.Key);
            }
            rmt2.DrawModes = new RenderMethodTemplate.RMT2PackedUInt16[num_drawmodes].ToList();

            PixelShader pixl;
            CachedTagInstance pixl_cachedtaginstance;

            using (var stream = CacheContext.OpenTagCacheRead())
            {
                pixl_cachedtaginstance = rmt2.PixelShader;
                pixl = (PixelShader)CacheContext.Deserializer.Deserialize(new TagSerializationContext(stream, CacheContext, pixl_cachedtaginstance), typeof(PixelShader));
            }

            // Reset pixel information
            pixl.DrawModes = new List<ShaderDrawMode>();
            pixl.Shaders = new List<PixelShaderBlock>();

            foreach (var result in shader_results)
            {
                var shader_mode = result.Key;
                var result_default = result.Value;
                var shader_mode_bitmask = (RenderMethodTemplate.ShaderModeBitmask)(1 << (int)shader_mode);
                rmt2.DrawModeBitmask |= shader_mode_bitmask;

                Register_Fixup.Register_Fixup_Manager manager = new Register_Fixup.Register_Fixup_Manager(CacheContext, result_default.Parameters, null);

                Register_Fixups_Textures_Samplers register_Fixups_Textures_Samplers = new Register_Fixups_Textures_Samplers();
                register_Fixups_Textures_Samplers.Fixup(manager);

                Register_Fixups_Arguments register_Fixups_Arguments = new Register_Fixups_Arguments();
                register_Fixups_Arguments.Fixup(manager);

                foreach (var target in manager.Targets)
                {
                    if (target.IsHandled)
                    {
                        Console.WriteLine($"SUCCESS: {target.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"FAILURE: {target.Name}");
                    }
                }

                foreach (var argument in manager.StringIDs_Arguments)
                {
                    rmt2.Arguments.Add(new RenderMethodTemplate.ShaderArgument
                    {
                        Name = CacheContext.GetStringId(argument)
                    });
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

                rmt2.DrawModeRegisterOffsets.Add(offset_block);
                

                var pixl_drawmode = new ShaderDrawMode();
                pixl_drawmode.Index = (byte)pixl.Shaders.Count();
                pixl_drawmode.Count = 1;

                var pixl_shaderblock = new PixelShaderBlock();
                pixl_shaderblock.PCParameters = result_default.Parameters;
                pixl_shaderblock.PCShaderBytecode = result_default.ByteCode;

                pixl.Shaders.Add(pixl_shaderblock);
                pixl.DrawModes.Add(pixl_drawmode);
            }

            using (var stream = CacheContext.TagCacheFile.Open(FileMode.Open, FileAccess.ReadWrite))
            {
                var context = new TagSerializationContext(stream, CacheContext, rmt2_cachedtaginstance);
                CacheContext.Serializer.Serialize(context, rmt2);
            }

            using (var stream = CacheContext.TagCacheFile.Open(FileMode.Open, FileAccess.ReadWrite))
            {
                var context = new TagSerializationContext(stream, CacheContext, pixl_cachedtaginstance);
                CacheContext.Serializer.Serialize(context, pixl);
            }

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