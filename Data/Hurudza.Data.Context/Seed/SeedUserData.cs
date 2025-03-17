using Hurudza.Data.Context.Context;
using Hurudza.Data.Context.Data;
using Hurudza.Data.Data;
using Hurudza.Data.Models.Models;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Hurudza.Data.Context.Seed;

public static class SeedUserData
{
    public static void SeedUsers(HurudzaDbContext dbContext, RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        if (dbContext.Users.Any(u => u.UserName == "admin@hurudza.co.zw")) return;
        
        var user = new ApplicationUser()
        {
            Email = "admin@hurudza.co.zw",
            Firstname = "System",
            Surname = "Administrator",
            PhoneNumber = "0773727342",
            UserName = "admin@hurudza.co.zw",
        };

        var result = userManager.CreateAsync(user, "Password+1").Result;

        if (!result.Succeeded) throw new Exception($"Failed to create user {user.UserName}");

        var role = roleManager.FindByNameAsync(ApiRoles.SystemAdministrator).Result;

        if (role == null)
            if (role != null)
                throw new Exception($"Role {role.Description} does not exist.");

        if (role != null)
        {
            var roleUser = userManager.AddToRoleAsync(user, role.Name).Result;

            if (!roleUser.Succeeded) throw new Exception($"Failed to add System Administrator to Role");
        }

        Log.Information("User created. {user}", user.UserName);
    }
}