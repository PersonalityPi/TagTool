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
using TagTool.Tags;

namespace TagTool.Commands.Shaders
{
    class CacheFlush : Command
    {
        private GameCacheContext CacheContext { get; }

        public CacheFlush(GameCacheContext cacheContext) :
            base(CommandFlags.Inherit,

                "CacheFlush",
                "Exports the tags.dat or sets the cache flushing mode",
                "CacheFlush <mode>",
                "Exports the tags.dat or sets the cache flushing mode")
        {
            CacheContext = cacheContext;
        }

        public override object Execute(List<string> args)
        {
            if(args.Count > 0)
            {
                var option = args[0].ToLower();
                switch (option)
                {
                    case "manual":
                        Console.WriteLine("Setting cache flushing mode to manual. Use CacheFlush without arguments to flush.");
                        CacheContext.TagCacheFile.AutomaticFlushing = false;

                        break;
                    case "automatic":
                        Console.WriteLine("Setting cache flushing mode to automatic.");
                        CacheContext.TagCacheFile.AutomaticFlushing = true;
                        break;
                    case "manual_resources":
                        Console.WriteLine("Setting resource cache flushing mode to manual. Use CacheFlush without arguments to flush.");
                        CacheContext.UseResourceAutomaticFlushing = false;

                        break;
                    case "automatic_resources":
                        Console.WriteLine("Setting resource cache flushing mode to automatic.");
                        CacheContext.UseResourceAutomaticFlushing = true;
                        break;
                    default:
                        Console.WriteLine($"Unknown option {option}");
                        break;
                }
            }
            else
            {
                Console.WriteLine($"{ CacheContext.TagCacheFile.FullName} written to disk");
                CacheContext.TagCacheFile.FlushData();

                CacheContext.StringIdCacheFile.FlushData();

                CacheContext.FlushResources();
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