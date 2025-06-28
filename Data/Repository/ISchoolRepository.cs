using System.Linq.Expressions;

namespace WebAPI2.Data.Repository;

public interface ISchoolRepository<T>
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Expression<Func<T?, bool>> filter, bool asNoTracking);
    Task<T?> GetByNameAsync(Expression<Func<T?, bool>> filter);
    Task<T> CreateAsync(T dbRecord);
    Task<T> Update(T dbRecord);
    bool Delete(T dbRecord);
}