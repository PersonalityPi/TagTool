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
    class GenerateAllRMT2 : Command
    {
        private GameCacheContext CacheContext { get; }

        public GenerateAllRMT2(GameCacheContext cacheContext) :
            base(CommandFlags.Inherit,

                "GenerateAllRMT2",
                "Finds all RMT2's and generates them again",
                "GenerateAllRMT2 <shader_type>",
                "Finds all RMT2's and generates them again")
        {
            CacheContext = cacheContext;
        }

        public override object Execute(List<string> args)
        {
            string check_template = null;
            if (args.Count > 0) check_template = args[0];


            var rmt2_cached_tags = CacheContext.TagCache.Index.NonNull().Where(t => TagDefinition.Find(t.Group.Tag) == typeof(RenderMethodTemplate)).ToList();

            foreach (var rmt2_cachedtaginstance in rmt2_cached_tags)
            {
                RenderMethodTemplate rmt2;
                using (var stream = CacheContext.OpenTagCacheRead())
                {
                    rmt2 = CacheContext.Deserializer.Deserialize<RenderMethodTemplate>(new TagSerializationContext(stream, CacheContext, rmt2_cachedtaginstance));
                }

                if (!CacheContext.TagNames.ContainsKey(rmt2_cachedtaginstance.Index)) continue;

                var name = CacheContext.TagNames[rmt2_cachedtaginstance.Index];

                var strings = name.Trim().Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                if (strings.Length < 3) continue;

                int[] template_args;
                string template;
                try
                {
                    template = strings[1];
                    template_args = strings[2].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                } catch
                {
                    continue;
                }

                if (check_template != null && template != check_template) continue;

                RMT2Generator generator = new RMT2Generator(CacheContext, rmt2, rmt2_cachedtaginstance, template, template_args);
                generator.Generate();
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