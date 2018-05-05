using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TagTool.Tags.Definitions.RenderMethodTemplate;

namespace TagTool.ShaderGenerator.RegisterFixups
{
    class Register_Fixups_Render_Method_Extern_Generic
    {
        bool FixupRegister()
        {



            return false; // Couldn't handle this register type
        }

        enum RenderMethodExtern
        {
            none = 0x00,
            texture_global_target_texaccum = 0x01,
            texture_global_target_normal = 0x02,
            texture_global_target_z = 0x03,
            texture_global_target_shadow_buffer1 = 0x04,
            texture_global_target_shadow_buffer2 = 0x05,
            texture_global_target_shadow_buffer3 = 0x06,
            texture_global_target_shadow_buffer4 = 0x07,
            texture_global_target_texture_camera = 0x08,
            texture_global_target_reflection = 0x09,
            texture_global_target_refraction = 0x0A,
            texture_lightprobe_texture = 0x0B,
            texture_dominant_light_intensity_map = 0x0C,
            texture_unused1 = 0x0D,
            texture_unused2 = 0x0E,
            object_change_color_primary = 0x0F,
            object_change_color_secondary = 0x10,
            object_change_color_tertiary = 0x11,
            object_change_color_quaternary = 0x12,
            object_change_color_quinary = 0x13,
            object_change_color_primary_anim = 0x14,
            object_change_color_secondary_anim = 0x15,
            texture_dynamic_environment_map_0 = 0x16,
            texture_dynamic_environment_map_1 = 0x17,
            texture_cook_torrance_cc0236 = 0x18,
            texture_cook_torrance_dd0236 = 0x19,
            texture_cook_torrance_c78d78 = 0x1A,
            light_dir_0 = 0x1B,
            light_color_0 = 0x1C,
            light_dir_1 = 0x1D,
            light_color_1 = 0x1E,
            light_dir_2 = 0x1F,
            light_color_2 = 0x20,
            light_dir_3 = 0x21,
            light_color_3 = 0x22,
            texture_unused_3 = 0x23,
            texture_unused_4 = 0x24,
            texture_unused_5 = 0x25,
            texture_dynamic_light_gel_0 = 0x26,
            flat_envmap_matrix_x = 0x27,
            flat_envmap_matrix_y = 0x28,
            flat_envmap_matrix_z = 0x29,
            debug_tint = 0x2A,
            screen_constants = 0x2B,
            active_camo_distortion_texture = 0x2C,
            scene_ldr_texture = 0x2D,
            scene_hdr_texture = 0x2E,
            water_memory_export_address = 0x2F,
            tree_animation_timer = 0x30,
            emblem_player_shoulder_texture = 0x31,
            emblem_clan_chest_texture = 0x32,
            unknown_index51 = 0x33
        };

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

        static Dictionary<RenderMethodExtern, List<string>> Arguments = new Dictionary<RenderMethodExtern, List<string>>
        {
            //{ RenderMethodExtern.none, new List<string> {} },
            { RenderMethodExtern.texture_global_target_texaccum, new List<string> {
                "albedo_texture",
                // Associated Properties
                "actually_calc_albedo",
                "g_exposure",
                "layers_of_4",
                "no_dynamic_lights",
                "order3_area_specular",
                "use_material_texture"
            } },
            { RenderMethodExtern.texture_global_target_normal, new List<string> {
                "normal_texture",
                // Associated Properties
                "g_exposure",
                "layers_of_4",
                "no_dynamic_lights",
                "order3_area_specular",
                "p_lighting_constant_0",
                "p_lighting_constant_1",
            } },
            { RenderMethodExtern.texture_global_target_z, new List<string> {
                "depth_buffer",
                // Associated Properties
                "g_exposure",
            } },
            { RenderMethodExtern.texture_global_target_shadow_buffer1, new List<string> {
                "shadow_depth_map_1",
                // Associated Properties
                "p_dynamic_light_gel_xform",
            } },
            { RenderMethodExtern.texture_global_target_shadow_buffer2, new List<string> {
                "shadow_depth_map_2", // Assumed
            } },
            { RenderMethodExtern.texture_global_target_shadow_buffer3, new List<string> {
                "shadow_depth_map_3", // Assumed
            } },
            { RenderMethodExtern.texture_global_target_shadow_buffer4, new List<string> {
                "shadow_depth_map_4", // Assumed
            } },
            //{ RenderMethodExtern.texture_global_target_texture_camera, new List<string> {} },
            //{ RenderMethodExtern.texture_global_target_reflection, new List<string> {} },
            //{ RenderMethodExtern.texture_global_target_refraction, new List<string> {} },
            { RenderMethodExtern.texture_lightprobe_texture, new List<string> {
                "lightprobe_texture_array",
                // Associated Properties
                "no_dynamic_lights",
            } },
            { RenderMethodExtern.texture_dominant_light_intensity_map, new List<string> {
                "dominant_light_intensity_map"
            } },
            //{ RenderMethodExtern.texture_unused1, new List<string> {} },
            //{ RenderMethodExtern.texture_unused2, new List<string> {} },
            //{ RenderMethodExtern.object_change_color_primary, new List<string> {} },
            //{ RenderMethodExtern.object_change_color_secondary, new List<string> {} },
            //{ RenderMethodExtern.object_change_color_tertiary, new List<string> {} },
            //{ RenderMethodExtern.object_change_color_quaternary, new List<string> {} },
            //{ RenderMethodExtern.object_change_color_quinary, new List<string> {} },
            //{ RenderMethodExtern.object_change_color_primary_anim, new List<string> {} },
            //{ RenderMethodExtern.object_change_color_secondary_anim, new List<string> {} },
            { RenderMethodExtern.texture_dynamic_environment_map_0, new List<string> {
                "dynamic_environment_map_0",
                // Associated Properties
                "texture_size",
                "k_ps_dominant_light_direction",
                "k_ps_dominant_light_intensity",
                // Associated Properties
                "actually_calc_albedo",
                "g_alt_exposure",
                "p_lighting_constant_1",
                "p_lighting_constant_2",
                "p_lighting_constant_3",
                "p_lighting_constant_4",
                "p_lighting_constant_5",
                "p_lighting_constant_6",
                "p_lighting_constant_7",
                "p_lighting_constant_8",
                "p_lighting_constant_9",
            } },
            { RenderMethodExtern.texture_dynamic_environment_map_1, new List<string> {
                "dynamic_environment_map_0",
                // Associated Properties
                "dynamic_environment_blend", // Controls blending between map 1 and map 2
                "texture_size",
                "k_ps_dominant_light_direction",
                "k_ps_dominant_light_intensity",
                // Associated Properties
                "actually_calc_albedo",
                "g_alt_exposure",
                //"p_lighting_constant_1", // Unverified, but this is missing from the output
                "p_lighting_constant_2",
                "p_lighting_constant_3",
                "p_lighting_constant_4",
                "p_lighting_constant_5",
                "p_lighting_constant_6",
                "p_lighting_constant_7",
                "p_lighting_constant_8",
                "p_lighting_constant_9",
            } },
            { RenderMethodExtern.texture_cook_torrance_cc0236, new List<string> {
                "g_sampler_cc0236",
                // Associated Properties
                "actually_calc_albedo",
                "g_alt_exposure",
                "k_ps_dominant_light_direction",
                "p_lighting_constant_2",
                "p_lighting_constant_3",
                "p_lighting_constant_4",
                "p_lighting_constant_5",
                "p_lighting_constant_6",
                "p_lighting_constant_7",
                "p_lighting_constant_8",
                "p_lighting_constant_9",
                "texture_size",
            } },
            { RenderMethodExtern.texture_cook_torrance_dd0236, new List<string> {
                "g_sampler_dd0236",
                // Associated Properties
                "actually_calc_albedo",
                "g_alt_exposure",
                "k_ps_dominant_light_direction",
                "k_ps_dominant_light_intensity",
                "p_lighting_constant_3",
                "p_lighting_constant_4",
                "p_lighting_constant_5",
                "p_lighting_constant_6",
                "p_lighting_constant_7",
                "p_lighting_constant_8",
                "p_lighting_constant_9",
                "texture_size",
            } },
            { RenderMethodExtern.texture_cook_torrance_c78d78, new List<string> {
                "g_sampler_c78d78",
                // Associated Properties
                "actually_calc_albedo",
                "g_alt_exposure",
                "k_ps_dominant_light_direction",
                "k_ps_dominant_light_intensity",
                "p_lighting_constant_4",
                "p_lighting_constant_5",
                "p_lighting_constant_6",
                "p_lighting_constant_7",
                "p_lighting_constant_8",
                "p_lighting_constant_9",
                "texture_size",
            } },
            //{ RenderMethodExtern.light_dir_0, new List<string> {} },
            //{ RenderMethodExtern.light_color_0, new List<string> {} },
            //{ RenderMethodExtern.light_dir_1, new List<string> {} },
            //{ RenderMethodExtern.light_color_1, new List<string> {} },
            //{ RenderMethodExtern.light_dir_2, new List<string> {} },
            //{ RenderMethodExtern.light_color_2, new List<string> {} },
            //{ RenderMethodExtern.light_dir_3, new List<string> {} },
            //{ RenderMethodExtern.light_color_3, new List<string> {} },
            //{ RenderMethodExtern.texture_unused_3, new List<string> {} },
            //{ RenderMethodExtern.texture_unused_4, new List<string> {} },
            //{ RenderMethodExtern.texture_unused_5, new List<string> {} },
            { RenderMethodExtern.texture_dynamic_light_gel_0, new List<string> {
                "dynamic_light_gel_texture",
                // Associated Properties
                "actually_calc_albedo",
                "p_dynamic_light_gel_xform",
            } },
            //{ RenderMethodExtern.flat_envmap_matrix_x, new List<string> {} },
            //{ RenderMethodExtern.flat_envmap_matrix_y, new List<string> {} },
            //{ RenderMethodExtern.flat_envmap_matrix_z, new List<string> {} },
            //{ RenderMethodExtern.debug_tint, new List<string> {} },
            //{ RenderMethodExtern.screen_constants, new List<string> {} },
            //{ RenderMethodExtern.active_camo_distortion_texture, new List<string> {} },
            { RenderMethodExtern.scene_ldr_texture, new List<string> {
                "scene_ldr_texture",
                // Associated Properties
                "g_exposure",
            } },
            //{ RenderMethodExtern.scene_hdr_texture, new List<string> {} },
            //{ RenderMethodExtern.water_memory_export_address, new List<string> {} },
            //{ RenderMethodExtern.tree_animation_timer, new List<string> {} },
            //{ RenderMethodExtern.emblem_player_shoulder_texture, new List<string> {} },
            //{ RenderMethodExtern.emblem_clan_chest_textur, new List<string> {} },
            { RenderMethodExtern.unknown_index51, new List<string> {
                "ibr_texture",
                // Associated Properties
                "actually_calc_albedo",
                "g_alt_exposure",
                "k_ps_dominant_light_direction",
                "p_lighting_constant_1",
                "p_lighting_constant_2",
                "p_lighting_constant_3",
                "p_lighting_constant_4",
                "p_lighting_constant_5",
                "p_lighting_constant_6",
                "p_lighting_constant_7",
                "p_lighting_constant_8",
                "p_lighting_constant_9",

            } }
        };
    }
}
