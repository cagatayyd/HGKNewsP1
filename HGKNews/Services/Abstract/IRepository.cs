using HGKNews.Entities;
using System.Linq.Expressions;

namespace HGKNews.Services.Abstract
{
    public interface IRepository<T> where T : class 
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllToListAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task EditAsync(int id, T entity);
        Task DeleteAsync(int id);
        Task<IQueryable<T>> WhereAsync(Func<T, bool> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<IQueryable<T>> OrderByAsync<TKey>(Expression<Func<T, TKey>> orderBy);
        Task<bool> SaveChangesAsync();
    }
}
