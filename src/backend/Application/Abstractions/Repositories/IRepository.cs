using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Abstractions.Repositories
{
    public interface IRepository<T> where T :  BaseEntity
    {
        Task<T> AddAsync(T entity);
        Task<List<T>> AddRangeAsync(List<T> entity);
        Task<T> UpdateAsync(T entity);
        Task<List<T>> UpdateRangeAsync(List<T> entities);


        Task<bool> DeleteAsync(T entity);
        Task<bool> DeleteRangeAsync(List<T> entity);

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> WhereAsync(Expression<Func<T, bool>> predicate);
        Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate);

    }
}
