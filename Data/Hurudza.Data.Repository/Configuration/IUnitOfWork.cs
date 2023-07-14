using Hurudza.Data.Repository.Interfaces;

namespace Musangano.Repository.Configuration;

public interface IUnitOfWork
{
    IUserRepository User { get; }

    Task CompleteAsync();

    void Dispose();
}