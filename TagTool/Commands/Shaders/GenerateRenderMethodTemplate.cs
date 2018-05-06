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

            RMT2Generator generator = new RMT2Generator(CacheContext, rmt2, rmt2_cachedtaginstance, shader_type, shader_args);
            generator.Generate();

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