using System.ComponentModel;

namespace Hurudza.Data.Enums.Enums
{
    public enum Terrain
    {
        [Description("Flat")]
        Flat = 1,
        [Description("Hilly")]
        Hilly,
        [Description("Mountainous")]
        Mountainous,
        [Description("Sloppy")]
        Sloppy
    }
}
