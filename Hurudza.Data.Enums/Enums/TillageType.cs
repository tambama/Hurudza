using System.ComponentModel;

namespace Hurudza.Data.Enums.Enums;

public enum TillageType
{
    [Description("Plowing")]
    Plowing = 1,
    
    [Description("Disking")]
    Disking,
    
    [Description("Harrowing")]
    Harrowing,
    
    [Description("Seeding")]
    Seeding,
    
    [Description("Ridging")]
    Ridging,
    
    [Description("Conservation Tillage")]
    ConservationTillage,
    
    [Description("Other")]
    Other
}