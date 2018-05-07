using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagTool.Cache;
using TagTool.Shaders;

namespace TagTool.ShaderGenerator.RegisterFixups
{
    abstract class Register_Fixup
    {
        public class Register_Fixup_Target
        {
            // Can't change these
            public string Name { get; internal set; }
            public ShaderParameter Parameter { get; internal set; }

            // These will be fixed up
            public int ArgumentIndex { get; set; }
            public bool IsHandled { get; set; }
            public bool IsHandledDirectly { get; set; }

            public Register_Fixup_Target(string name, ShaderParameter parameter)
            {
                Name = name;
                Parameter = parameter;
            }
        }

        public class Register_Fixup_Manager
        {
            public GameCacheContext CacheContext { get; }
            public List<Register_Fixup_Target> Targets { get; } = new List<Register_Fixup_Target>();

            public List<Register_Fixup_Target> Targets_Textures_Samplers = new List<Register_Fixup_Target>();
            public List<Register_Fixup_Target> Targets_Arguments = new List<Register_Fixup_Target>();
            public List<Register_Fixup_Target> Targets_Global_Arguments = new List<Register_Fixup_Target>();
            public List<Register_Fixup_Target> Targets_Halogram_or_Empty_Targets = new List<Register_Fixup_Target>();
            public List<Register_Fixup_Target> Targets_Render_Method_Extern_Generic_Targets = new List<Register_Fixup_Target>();
            public List<Register_Fixup_Target> Targets_Render_Method_Extern_Vectors_Targets = new List<Register_Fixup_Target>();
            public List<Register_Fixup_Target> Targets_Water_Targets = new List<Register_Fixup_Target>();

            public List<string> StringIDs_Arguments = new List<string>();
            public List<string> StringIDs_Unknown5 = new List<string>();
            public List<string> StringIDs_GlobalArguments = new List<string>();
            public List<string> StringIDs_Textures_Samplers = new List<string>();

            public Register_Fixup_Manager(GameCacheContext cache_context, List<ShaderParameter> parameters, Register_Fixup_Manager last_iteration)
            {
                CacheContext = cache_context;

                foreach(var parameter in parameters)
                {
                    var fixup = new Register_Fixup_Target(CacheContext.GetString(parameter.ParameterName), parameter);
                    Targets.Add(fixup);
                }

                if(last_iteration != null)
                {
                    StringIDs_Arguments = last_iteration.StringIDs_Textures_Samplers;
                    StringIDs_Unknown5 = last_iteration.StringIDs_Unknown5;
                    StringIDs_GlobalArguments = last_iteration.StringIDs_GlobalArguments;
                    StringIDs_Textures_Samplers = last_iteration.StringIDs_Textures_Samplers;
                }

            }
        }

        public abstract void Fixup(Register_Fixup_Manager manager);
    }
}
