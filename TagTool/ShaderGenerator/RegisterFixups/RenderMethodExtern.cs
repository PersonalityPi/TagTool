using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagTool.ShaderGenerator.RegisterFixups
{
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
}
