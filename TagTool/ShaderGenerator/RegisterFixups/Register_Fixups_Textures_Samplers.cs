using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagTool.Shaders;

namespace TagTool.ShaderGenerator.RegisterFixups
{
    class Register_Fixups_Textures_Samplers : Register_Fixup
    {
        /*
         * In this fixup we just want to generate the StringID's for the corresponding
         * arguments and fill them into the RMT2's arguments array
         * 
         * We should be able to skip this step in the shader compilation stage
         * as we're already generating that information. However this list is considerably
         * more maintainable than each specific template list
         * 
         */

        public override void Fixup(Register_Fixup_Manager manager)
        {
            foreach (var target in manager.Targets)
            {
                if (target.IsHandled) continue;

                var index = Arguments.ContainsKey(target.Name);
                if (index)
                {
                    if (target.Parameter.RegisterIndex >= 16) throw new Exception("Invalid argument index. Maximum of 16 samplers");

                    var argument_type = Arguments[target.Name];
                    if(target.Parameter.RegisterType != argument_type) throw new Exception($"Incorrect type for {target.Name}");

                    target.IsHandledDirectly = target.Parameter.RegisterType == ShaderParameter.RType.Sampler;
                    if(target.IsHandledDirectly)
                    {
                        //NOTE: We only need to add textures in here, other types other than samplers are invalid
                        // and we're only including other associated data as an optimization which appears to have
                        // been made to this type of data

                        //NOTE: We should check that StringID's always match up 1:1 with their names
                        if (!manager.StringIDs_Textures_Samplers.Contains(target.Name))
                            manager.StringIDs_Textures_Samplers.Add(target.Name);
                        target.ArgumentIndex = manager.StringIDs_Textures_Samplers.IndexOf(target.Name);
                    }

                    // Add this to the textures_samplers
                    manager.Targets_Textures_Samplers.Add(target);

                    target.IsHandled = true;
                    
                }
            }
        }

        Dictionary<string, ShaderParameter.RType> Arguments = new Dictionary<string, ShaderParameter.RType>
        {
            // Associated Values. Not sure why these are in here actually.
            // But it doesn't appear to be a single template either.
            { "order3_area_specular", ShaderParameter.RType.Boolean },
            { "no_dynamic_lights", ShaderParameter.RType.Boolean },
            { "actually_calc_albedo", ShaderParameter.RType.Boolean },
            { "layers_of_4", ShaderParameter.RType.Integer },
            { "g_exposure", ShaderParameter.RType.Vector },
            { "g_alt_exposure", ShaderParameter.RType.Vector },
            { "k_ps_dominant_light_direction", ShaderParameter.RType.Vector },
            { "p_dynamic_light_gel_xform", ShaderParameter.RType.Vector },
            { "p_lighting_constant_0", ShaderParameter.RType.Vector },
            { "p_lighting_constant_1", ShaderParameter.RType.Vector },
            { "p_lighting_constant_2", ShaderParameter.RType.Vector },
            { "p_lighting_constant_3", ShaderParameter.RType.Vector },
            { "p_lighting_constant_4", ShaderParameter.RType.Vector },
            { "p_lighting_constant_5", ShaderParameter.RType.Vector },
            { "p_lighting_constant_6", ShaderParameter.RType.Vector },
            { "p_lighting_constant_8", ShaderParameter.RType.Vector },
            { "p_lighting_constant_7", ShaderParameter.RType.Vector },
            { "p_lighting_constant_9", ShaderParameter.RType.Vector },

            // Samplers
            { "base_map", ShaderParameter.RType.Sampler },
            { "palette", ShaderParameter.RType.Sampler },
            { "alpha_map", ShaderParameter.RType.Sampler },
            { "detail_map", ShaderParameter.RType.Sampler },
            { "bump_map", ShaderParameter.RType.Sampler },
            { "alpha_test_map", ShaderParameter.RType.Sampler },
            { "multiply_map", ShaderParameter.RType.Sampler },
            { "change_color_map", ShaderParameter.RType.Sampler },
            { "vector_map", ShaderParameter.RType.Sampler },
            { "self_illum_map", ShaderParameter.RType.Sampler },
            { "overlay_map", ShaderParameter.RType.Sampler },
            { "overlay_detail_map", ShaderParameter.RType.Sampler },
            { "noise_map_a", ShaderParameter.RType.Sampler },
            { "noise_map_b", ShaderParameter.RType.Sampler },
            { "alpha_mask_map", ShaderParameter.RType.Sampler },
            { "self_illum_detail_map", ShaderParameter.RType.Sampler },
            { "overlay_multiply_map", ShaderParameter.RType.Sampler },
            { "warp_map", ShaderParameter.RType.Sampler },
            { "detail_map2", ShaderParameter.RType.Sampler },
            { "detail_map_overlay", ShaderParameter.RType.Sampler },
            { "detail_map_a", ShaderParameter.RType.Sampler },
            { "detail_mask_a", ShaderParameter.RType.Sampler },
            { "height_map", ShaderParameter.RType.Sampler },
            { "environment_map", ShaderParameter.RType.Sampler },
            { "material_texture", ShaderParameter.RType.Sampler },
            { "meter_map", ShaderParameter.RType.Sampler },
            { "specular_mask_texture", ShaderParameter.RType.Sampler },
            { "bump_detail_map", ShaderParameter.RType.Sampler },
            { "bump_detail_mask_map", ShaderParameter.RType.Sampler },
            { "chameleon_mask_map", ShaderParameter.RType.Sampler },
            { "color_mask_map", ShaderParameter.RType.Sampler },
            { "detail_map3", ShaderParameter.RType.Sampler },
            { "blend_map", ShaderParameter.RType.Sampler },
            { "base_map_m_0", ShaderParameter.RType.Sampler },
            { "detail_map_m_0", ShaderParameter.RType.Sampler },
            { "bump_map_m_0", ShaderParameter.RType.Sampler },
            { "detail_bump_m_0", ShaderParameter.RType.Sampler },
            { "base_map_m_1", ShaderParameter.RType.Sampler },
            { "detail_map_m_1", ShaderParameter.RType.Sampler },
            { "bump_map_m_1", ShaderParameter.RType.Sampler },
            { "detail_bump_m_1", ShaderParameter.RType.Sampler },
            { "base_map_m_2", ShaderParameter.RType.Sampler },
            { "detail_map_m_2", ShaderParameter.RType.Sampler },
            { "bump_map_m_2", ShaderParameter.RType.Sampler },
            { "detail_bump_m_2", ShaderParameter.RType.Sampler },
            { "base_map_m_3", ShaderParameter.RType.Sampler },
            { "detail_map_m_3", ShaderParameter.RType.Sampler },
            { "bump_map_m_3", ShaderParameter.RType.Sampler },
            { "detail_bump_m_3", ShaderParameter.RType.Sampler },
            { "lightprobe_texture_array", ShaderParameter.RType.Sampler },
            { "wave_slope_array", ShaderParameter.RType.Sampler },
            { "foam_texture", ShaderParameter.RType.Sampler },
            { "foam_texture_detail", ShaderParameter.RType.Sampler },
            { "scene_ldr_texture", ShaderParameter.RType.Sampler },
            { "watercolor_texture", ShaderParameter.RType.Sampler },
            { "global_shape_texture", ShaderParameter.RType.Sampler },
        };
    }
}
