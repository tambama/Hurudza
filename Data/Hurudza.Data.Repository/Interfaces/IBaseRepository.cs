using System.Linq.Expressions;

namespace Hurudza.Data.Repository.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task<IEnumerable<T>> All();
    Task<T> GetById(string id);
    Task<bool> Add(T entity);
    Task<bool> Delete(string id);
    Task<bool> Upsert(T entity);
    Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate);
}