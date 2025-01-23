using MicroServices.Shared.Adapters;
using MicroServices.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MicroServices.Shared.DataAccess
{
    public interface IGenericRepository<T> where T : AuditableEntity
    {
        DbSet<T> Entity { get; }
        IDateTimeAdapter DateTimeAdapter { get; }

        Task<long> AddAsync(T entity);
        Task<long> AddRangeAsync(List<T> entities);
        Task<long> CountAsync(Expression<Func<T, bool>> filter, bool ignoreFilters = false);
        Task<bool> ExistAsync(Expression<Func<T, bool>> filter, bool ignoreFilters = false);
        Task<bool> DeleteAsync(T entity, bool isSoftDelete = true);
        Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> filter, bool ignoreFilters = false, Expression<Func<T, object>> includeProperties = null, string includePropertiesAsString = null);
        Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> filter, int pageNumber, int pageSize, bool ignoreFilters = false, Expression<Func<T, object>> includeProperties = null, string includePropertiesAsString = null);
        Task<IQueryable<NewT>> GetAllAsync<NewT>(Expression<Func<T, bool>> filter, Expression<Func<T, NewT>> select, bool ignoreFilters = false, Expression<Func<T, object>> includeProperties = null, string includePropertiesAsString = null);
        Task<T> GetAsync(Expression<Func<T, bool>> filter, bool ignoreFilters = false, Expression<Func<T, object>> includeProperties = null, string includePropertiesAsString = null);
        Task<NewT> GetAsync<NewT>(Expression<Func<T, bool>> filter, Expression<Func<T, NewT>> select, bool ignoreFilters = false, Expression<Func<T, object>> includeProperties = null, string includePropertiesAsString = null);
        Task<T> GetByIdAsync(long entityId, bool ignoreFilters = false, Expression<Func<T, object>> includeProperties = null);
        Task<NewT> GetByIdAsync<NewT>(long entityId, Expression<Func<T, NewT>> select, bool ignoreFilters = false, Expression<Func<T, object>> includeProperties = null);
        Task<List<T>> GetListAsync(bool ignoreFilters = false);
        Task<int> SqlQuery(string query);
        Task<bool> UpdateAsync(T entity);
        Task<bool> UpdateRangeAsync(List<T> entities);
    }
}