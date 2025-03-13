using System.Security.Claims;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Context.Data;
using Hurudza.Data.Models.Models;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Hurudza.Data.Context.Seed
{
    public static class SeedRoleData
    {
        public static void SeedRoles(HurudzaDbContext dbContext, RoleManager<ApplicationRole> roleManager)
        {
            var roles = ApplicationRoles.GetApplicationRoles();

            foreach (var role in roles)
            {
                if (role.Name == null) continue;
                
                var r = roleManager.FindByNameAsync(role.Name).Result;

                if (r == null)
                {
                    var result = roleManager.CreateAsync(new ApplicationRole
                        { Name = role.Name, Description = role.Description, RoleClass = role.RoleClass }).Result;

                    if (!result.Succeeded) throw new Exception($"Failed to Create Role {role.Description}");

                    r = roleManager.FindByNameAsync(role.Name).Result;

                    if (role.RoleClaims
                        .Select(claim =>
                            roleManager.AddClaimAsync(r, new Claim(claim.ClaimType, claim.ClaimValue)).Result)
                        .Any(rResult => !rResult.Succeeded))
                    {
                        throw new Exception("Failed to add Role Claim");
                    }
                }
                else
                {
                    var claims = dbContext.RoleClaims.Where(c => c.RoleId == r.Id).ToList();
                    var totalClaims = claims.Count;

                    if (totalClaims == role.RoleClaims.Count) continue;

                    if (totalClaims != 0)
                    {
                        dbContext.RemoveRange(claims);
                        dbContext.SaveChanges();
                    }

                    if (role.RoleClaims
                        .Select(claim =>
                            roleManager.AddClaimAsync(r, new Claim(claim.ClaimType, claim.ClaimValue)).Result)
                        .Any(rResult => !rResult.Succeeded))
                    {
                        throw new Exception("Failed to add Role Claim");
                    }
                }
            }
        }
    }
}