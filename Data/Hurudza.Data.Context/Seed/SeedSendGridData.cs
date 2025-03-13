using Hurudza.Data.Context.Context;
using Hurudza.Data.Context.Data;

namespace Hurudza.Data.Context.Seed;

public static class SeedSendGridData
{
    public static void SeedSendGridTemplates(HurudzaDbContext dbContext)
    {
        if (dbContext.SendGridTemplates == null ||
            dbContext.SendGridTemplates.Count() == SendGridTemplates.GetTemplates().Count) return;
            
        dbContext.RemoveRange(dbContext.SendGridTemplates.ToList());
        dbContext.SaveChanges();

        dbContext.AddRange(SendGridTemplates.GetTemplates());
        dbContext.SaveChanges();
    }
}