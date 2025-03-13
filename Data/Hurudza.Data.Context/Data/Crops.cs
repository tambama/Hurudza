using Hurudza.Data.Models.Models;

namespace Hurudza.Data.Context.Data;

public static class Crops
{
    public static List<Crop> GetCrops()
    {
        return new List<Crop>
        {
            new() { Name = "Maize" },
            new() { Name = "Wheat" },
            new() { Name = "Beans" },
        };
    }
}