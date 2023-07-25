using Hurudza.Data.Context.Context;
using Hurudza.Data.Context.Data;

namespace Hurudza.Data.Context.Seed;

public static class SeedClaimsData
{
    public static void SeedClaims(HurudzaDbContext context)
    {
        var newClaims = ApiClaims.GetClaims();

        var currentClaims = context.Claims.ToList();

        var nonIntersectionItems = newClaims.Select(c => c.ClaimValue)
            .Except(currentClaims.Select(c => c.ClaimValue))
            .Count();

        if (newClaims.Count == currentClaims.Count && nonIntersectionItems <= 0) return;
        
        context.Claims.RemoveRange(currentClaims);
        context.Claims.AddRange(newClaims);
        context.SaveChanges();
    }
}