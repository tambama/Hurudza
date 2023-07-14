using Hurudza.Data.Data.Context;
using Hurudza.Data.Repository.Interfaces;
using Hurudza.Data.Repository.Repositories;
using Microsoft.Extensions.Configuration;
using Musangano.Repository.Configuration;

namespace Hurudza.Data.Repository.Configuration;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly HurudzaDbContext _context;
    private readonly IConfiguration _configuration;

    public IUserRepository User { get; private set; }

    public UnitOfWork(HurudzaDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        User = new UserRepository(context);
    }

    public async Task CompleteAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}