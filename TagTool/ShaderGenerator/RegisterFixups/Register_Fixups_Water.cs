using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagTool.ShaderGenerator.RegisterFixups
{
    class Register_Fixups_Water
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

        Dictionary<string, int> Arguments = new Dictionary<string, int>
        {
            {"time_warp", 3 },
            {"simple_light_count", 5 },
            {"simple_lights", 28 },
            // Just assume that this will be fine, probbaly not though
            //{"simple_lights", 29 },
            //{"simple_lights", 30 },
        };
    }
}
