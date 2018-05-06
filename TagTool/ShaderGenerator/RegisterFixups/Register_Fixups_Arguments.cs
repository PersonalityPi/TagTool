using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagTool.ShaderGenerator.RegisterFixups
{
    class Register_Fixups_Arguments : Register_Fixup
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
            foreach(var target in manager.Targets)
            {
                var index = Arguments.IndexOf(target.Name);
                if(index != -1)
                {
                    if (target.Parameter.RegisterIndex < 58) throw new Exception("Invalid argument index. Must be greater than 58!");
                    if (target.Parameter.RegisterIndex > 100) throw new Exception("Invalid argument index. Overwriting statis structures, index must be less than 100!");

                    //NOTE: Perhaps we should add this.....
                    //var argument_type = Arguments[target.Name];
                    //if (target.Parameter.RegisterType != argument_type) throw new Exception($"Incorrect type for {target.Name}");

                    //NOTE: We should check that StringID's always match up 1:1 with their names
                    if (!manager.StringIDs_Arguments.Contains(target.Name))
                        manager.StringIDs_Arguments.Add(target.Name);
                    target.ArgumentIndex = manager.StringIDs_Textures_Samplers.IndexOf(target.Name);

                    // Add this to the textures_samplers
                    manager.Targets_Arguments.Add(target);

                    target.IsHandled = true;
                }
            }
        }

        List<string> Arguments = new List<string>
        {
            "albedo_color",
            "add_color",
            "albedo_blend",
            "albedo_blend_with_specular_tint",
            "albedo_color2",
            "albedo_color3",
            "albedo_specular_tint_blend",
            "albedo_specular_tint_blend_m_0",
            "albedo_specular_tint_blend_m_1",
            "albedo_specular_tint_blend_m_2",
            "albedo_specular_tint_blend_m_3",
            "alpha_map_xform",
            "alpha_mask_map_xform",
            "alpha_test_map_xform",
            "analytical_anti_shadow_control",
            "analytical_specular_contribution",
            "analytical_specular_contribution_m_0",
            "analytical_specular_contribution_m_1",
            "analytical_specular_contribution_m_2",
            "analytical_specular_contribution_m_3",
            "antialias_tweak",
            "area_specular_contribution",
            "area_specular_contribution_m_0",
            "area_specular_contribution_m_1",
            "area_specular_contribution_m_2",
            "area_specular_contribution_m_3",
            "bankalpha_infuence_depth",
            "base_map_m_0_xform",
            "base_map_m_1_xform",
            "base_map_m_2_xform",
            "base_map_m_3_xform",
            "base_map_xform",
            "blend_map_xform",
            "bump_detail_coefficient",
            "bump_detail_map_xform",
            "bump_detail_mask_map_xform",
            "bump_map_m_0_xform",
            "bump_map_m_1_xform",
            "bump_map_m_2_xform",
            "bump_map_m_3_xform",
            "bump_map_xform",
            "chameleon_color_offset1",
            "chameleon_color_offset2",
            "chameleon_color0",
            "chameleon_color1",
            "chameleon_color2",
            "chameleon_color3",
            "chameleon_fresnel_power",
            "chameleon_mask_map_xform",
            "change_color_map_xform",
            "channel_a",
            "channel_b",
            "channel_c",
            "color_mask_map_xform",
            "color_medium",
            "color_sharp",
            "color_wide",
            "depth_darken",
            "depth_fade_range",
            "detail_bump_m_0_xform",
            "detail_bump_m_1_xform",
            "detail_bump_m_2_xform",
            "detail_bump_m_3_xform",
            "detail_fade_a",
            "detail_map_a_xform",
            "detail_map_m_0_xform",
            "detail_map_m_1_xform",
            "detail_map_m_2_xform",
            "detail_map_m_3_xform",
            "detail_map_overlay_xform",
            "detail_map_xform",
            "detail_map2_xform",
            "detail_map3_xform",
            "detail_mask_a_xform",
            "detail_multiplier_a",
            "detail_slope_scale_x",
            "detail_slope_scale_y",
            "detail_slope_scale_z",
            "detail_slope_steepness",
            "diffuse_coefficient",
            "diffuse_coefficient_m_0",
            "diffuse_coefficient_m_1",
            "diffuse_coefficient_m_2",
            "diffuse_coefficient_m_3",
            "distortion_scale",
            "edge_fade_center_tint",
            "edge_fade_edge_tint",
            "edge_fade_power",
            "env_roughness_scale",
            "env_tint_color",
            "environment_map_specular_contribution",
            "environment_specular_contribution_m_0",
            "environment_specular_contribution_m_1",
            "environment_specular_contribution_m_2",
            "environment_specular_contribution_m_3",
            "foam_height",
            "foam_pow",
            "foam_texture_detail_xform",
            "foam_texture_xform",
            "fresnel_coefficient",
            "fresnel_color",
            "fresnel_color_environment",
            "fresnel_curve_bias",
            "fresnel_curve_steepness",
            "fresnel_curve_steepness_m_0",
            "fresnel_curve_steepness_m_1",
            "fresnel_curve_steepness_m_2",
            "fresnel_curve_steepness_m_3",
            "fresnel_power",
            "glancing_specular_power",
            "glancing_specular_tint",
            "global_albedo_tint",
            "height_map_xform",
            "height_scale",
            "intensity",
            "layer_contrast",
            "layer_depth",
            "layers_of_4",
            "material_texture_xform",
            "meter_color_off",
            "meter_color_on",
            "meter_map_xform",
            "meter_value",
            "minimal_wave_disturbance",
            "modulation_factor",
            "multiply_map_xform",
            "neutral_gray",
            "no_dynamic_lights",
            "noise_map_a_xform",
            "noise_map_b_xform",
            "normal_specular_power",
            "normal_specular_tint",
            "order3_area_specular",
            "overlay_detail_map_xform",
            "overlay_intensity",
            "overlay_map_xform",
            "overlay_multiply_map_xform",
            "overlay_tint",
            "primary_change_color_blend",
            "reflection_coefficient",
            "refraction_depth_dominant_ratio",
            "refraction_extinct_distance",
            "refraction_texcoord_shift",
            "rim_fresnel_albedo_blend",
            "rim_fresnel_coefficient",
            "rim_fresnel_color",
            "rim_fresnel_power",
            "roughness",
            "self_illum_color",
            "self_illum_detail_map_xform",
            "self_illum_heat_color",
            "self_illum_intensity",
            "self_illum_map_xform",
            "shadow_intensity_mark",
            "slope_range_x",
            "slope_range_y",
            "specular_coefficient",
            "specular_coefficient_m_0",
            "specular_coefficient_m_1",
            "specular_coefficient_m_2",
            "specular_coefficient_m_3",
            "specular_mask_texture_xform",
            "specular_power_m_0",
            "specular_power_m_1",
            "specular_power_m_2",
            "specular_power_m_3",
            "specular_tint",
            "specular_tint_m_0",
            "specular_tint_m_1",
            "specular_tint_m_2",
            "specular_tint_m_3",
            "sunspot_cut",
            "texcoord_aspect_ratio",
            "thinness_medium",
            "thinness_sharp",
            "thinness_wide",
            "time_warp",
            "time_warp_aux",
            "tint_color",
            "use_fresnel_color_environment",
            "vector_sharpness",
            "warp_amount",
            "warp_amount_x",
            "warp_amount_y",
            "warp_map_xform",
            "water_color_pure",
            "water_diffuse",
            "water_murkiness",
            "watercolor_coefficient",
            "wave_displacement_array_xform",
            "wave_slope_array_xform",

        };
    }
}
