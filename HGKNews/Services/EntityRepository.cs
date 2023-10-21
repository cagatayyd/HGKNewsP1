using Microsoft.EntityFrameworkCore;
using HGKNews.Context;
using HGKNews.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HGKNews.Entities;
using System.Linq.Expressions;

namespace HGKNews.Services
{
    public class EntityRepository<T> : IRepository<T> where T : class
    {
        #region Fields

        private readonly NewsDbContext _dbContext;

        #endregion

        #region Ctor

        public EntityRepository(NewsDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        #endregion

        public virtual async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await _dbContext.Set<T>().FindAsync(id);
            if (entity != null)
            {
                _dbContext.Set<T>().Remove(entity);
            }
        }
        public virtual async Task EditAsync(int id, T entity)
        {
            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<IQueryable<T>> WhereAsync(Func<T, bool> predicate)
        {
            return (IQueryable<T>) await Task.FromResult(_dbContext.Set<T>().Where(predicate));
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }


        public virtual async Task<bool> SaveChangesAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllToListAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }


        public virtual async Task<IQueryable<T>> OrderByAsync<TKey>(Expression<Func<T, TKey>> orderBy)
        {
            var query = _dbContext.Set<T>().OrderBy(orderBy);
            return await Task.FromResult(query);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().CountAsync(predicate);
        }
    }
}
