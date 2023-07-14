using Hurudza.Data.Data.Context;
using Hurudza.Data.Models.Models;
using Hurudza.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Hurudza.Data.Repository.Repositories;

public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
{
    public UserRepository(HurudzaDbContext context) : base(context)
    {
    }

    public override async Task<bool> Upsert(ApplicationUser entity)
    {
        try
        {
            var existingUser = await DbSet.Where(x => x.Id == entity.Id)
                .FirstOrDefaultAsync();

            if (existingUser == null)
                return await Add(entity);

            existingUser.Email = entity.Email;
            existingUser.PhoneNumber = entity.PhoneNumber;

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "{Repo} Upsert function error", typeof(UserRepository));
            return false;
        }
    }
}