using System.Linq.Expressions;
using Hurudza.Data.Data.Context;
using Hurudza.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Hurudza.Data.Repository.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected HurudzaDbContext Context;
    internal readonly DbSet<T> DbSet;

    protected BaseRepository(HurudzaDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> All()
    {
        try
        {
            return await DbSet.ToListAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "{Repo} All function error", typeof(UserRepository));
            return new List<T>();
        }
    }

    public virtual async Task<T> GetById(string id)
    {
        return await DbSet.FindAsync(id);
    }

    public virtual async Task<bool> Add(T entity)
    {
        await DbSet.AddAsync(entity);
        return true;
    }

    public virtual async Task<bool> Delete(string id)
    {
        try
        {
            var exist = await DbSet.FindAsync(id);

            if (exist == null) return false;

            DbSet.Remove(exist);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "{Repo} Delete function error", typeof(T));
            return false;
        }
    }

    public virtual Task<bool> Upsert(T entity)
    {
        DbSet.Update(entity);
        return Task.FromResult(true);
    }

    public virtual async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.Where(predicate).ToListAsync();
    }
}