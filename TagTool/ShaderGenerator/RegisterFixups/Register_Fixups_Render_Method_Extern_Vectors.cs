using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TagTool.Tags.Definitions.RenderMethodTemplate;

namespace TagTool.ShaderGenerator.RegisterFixups
{
    class Register_Fixups_Render_Method_Extern_Vectors : Register_Fixup
    {

        public override void Fixup(Register_Fixup_Manager manager)
        {
            
        }

        class Render_Method_Extern_Fixup
        {
            public string Name { get; internal set; }
            public RenderMethodExtern Argument { get; internal set; }
            public ShaderModeBitmask AllowedModes { get; internal set; }
            public ShaderModeBitmask IgnoredModes { get; internal set; }

            public Render_Method_Extern_Fixup(string name, RenderMethodExtern argument, ShaderModeBitmask allowed_modes = ShaderModeBitmask.None, ShaderModeBitmask ignored_modes = ShaderModeBitmask.None)
            {
                Name = name;
                Argument = argument;
                AllowedModes = allowed_modes;
                IgnoredModes = ignored_modes;
            }
        }

        static List<Render_Method_Extern_Fixup> Arguments = new List<Render_Method_Extern_Fixup>
        {
            new Render_Method_Extern_Fixup("primary_change_color", RenderMethodExtern.object_change_color_primary ), // Verified
            new Render_Method_Extern_Fixup("secondary_change_color", RenderMethodExtern.object_change_color_secondary ), // Verified
            new Render_Method_Extern_Fixup("tertiary_change_color", RenderMethodExtern.object_change_color_tertiary ), // Verified
            new Render_Method_Extern_Fixup("quaternary_change_color", RenderMethodExtern.object_change_color_quaternary ), // Verified
            new Render_Method_Extern_Fixup("quinary_change_color", RenderMethodExtern.object_change_color_quinary ), // Assumed

            new Render_Method_Extern_Fixup("primary_change_color_anim", RenderMethodExtern.object_change_color_primary_anim ), // Verified
            new Render_Method_Extern_Fixup("secondary_change_color_anim", RenderMethodExtern.object_change_color_secondary_anim ), // Verified
            
            new Render_Method_Extern_Fixup("debug_tint", RenderMethodExtern.debug_tint ), // Verified
            new Render_Method_Extern_Fixup("screen_constants", RenderMethodExtern.screen_constants ), // Verified

            // self_illum_color appears to just be an internal mapping to quinary_change_color
            new Render_Method_Extern_Fixup("self_illum_color", RenderMethodExtern.object_change_color_quaternary ), // Verified
        };
    }
}
