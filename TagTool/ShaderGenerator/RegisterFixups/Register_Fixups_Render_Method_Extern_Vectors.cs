using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TagTool.Tags.Definitions.RenderMethodTemplate;

namespace TagTool.ShaderGenerator.RegisterFixups
{
    class Register_Fixups_Render_Method_Extern_Vectors
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
            emblem_clan_chest_textur = 0x32,
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
