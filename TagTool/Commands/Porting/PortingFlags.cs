using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagTool.Commands.Porting
{
    [Flags]
    public enum PortingFlags
    {
        None,
        Replace = 1 << 0,
        Recursive = 1 << 1,
        New = 1 << 2,
        UseNull = 1 << 3,
        NoAudio = 1 << 4,
        NoElites = 1 << 5,
        NoForgePalette = 1 << 6,
        NoSquads = 1 << 7,
        Scripts = 1 << 8,
        ShaderTest = 1 << 9,
        MatchShaders = 1 << 10,
        Memory = 1 << 11,
        NoRmhg = 1 << 12
    }
}
