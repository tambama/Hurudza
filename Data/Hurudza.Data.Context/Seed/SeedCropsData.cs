using Hurudza.Data.Context.Context;
using Hurudza.Data.Context.Data;
using Microsoft.EntityFrameworkCore;

namespace Hurudza.Data.Context.Seed;

public static class SeedCropsData
{
    public static async Task SeedCrops(HurudzaDbContext dbContext)
    {
        var newCrops = Crops.GetCrops();
        var currentCrops = await dbContext.Crops.ToListAsync();

        if (newCrops.Count != currentCrops.Count)
        {
            dbContext.RemoveRange(currentCrops);
            await dbContext.AddRangeAsync(newCrops);
            await dbContext.SaveChangesAsync();
        }
    }
}