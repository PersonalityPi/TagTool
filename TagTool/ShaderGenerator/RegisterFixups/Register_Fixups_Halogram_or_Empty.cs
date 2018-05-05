using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagTool.ShaderGenerator.RegisterFixups
{
    class Register_Fixups_Halogram_or_Empty
    {
        /*
         * Not 100% sure what this is yet, it appears to only be in Halograms
         * I have a suspicion that these are things that aren't found from globals.
         * 
         * These tags reference layers_of_4 from globals, but perhaps in this template
         * you can't use that, so I think this source just sets them to null.
         * 
         * For now, we'll only apply this to Halogram templates until we properly
         * identify everything.
         * 
         */

        List<string> Arguments = new List<string>
        {
            "g_exposure",
            "layers_of_4",
            "self_illum_map",
            "albedo_texture",
            "normal_texture",
            "warp_map",
        };
    }
}
