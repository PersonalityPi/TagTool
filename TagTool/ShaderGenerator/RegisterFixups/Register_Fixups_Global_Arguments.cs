using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagTool.ShaderGenerator.RegisterFixups
{
    class Register_Fixups_Global_Arguments : Register_Fixup
    {
        public override void Fixup(Register_Fixup_Manager manager)
        {
            
        }

        static Dictionary<string, List<string>> Arguments = new Dictionary<string, List<string>>
        {
            { "no_dynamic_lights", new List<string> {
                "no_dynamic_lights",
                // Associated Properties
                //TODO obscured data, unlikely to be all of them
            } },
            { "order3_area_specular", new List<string> {
                "order3_area_specular",
                // Associated Properties
                //TODO this might not be all of them
                "albedo_texture",
                "environment_map",
                "g_exposure",
                "normal_texture",
                "p_lighting_constant_0",
                "use_material_texture",
            } },
            { "use_material_texture", new List<string> {
                "no_dynamic_lights",
                // Associated Properties
                "normal_texture",
                "lightprobe_texture_array",
                "base_map",
                "material_texture",
                "height_map",
                "self_illum_map",
                "specular_mask_texture",
                "bump_map",
                "p_lighting_constant_1"
            } },
        };
    }
}
