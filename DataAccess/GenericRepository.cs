using MicroServices.Shared.Adapters;
using MicroServices.Shared.Extensions;
using MicroServices.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MicroServices.Shared.DataAccess
{
    public class GenericRepository<T> : IGenericRepository<T> where T : AuditableEntity
    {
        private readonly DbContext _dbContext;
        private readonly CurrentUser _user;
        public DbSet<T> Entity { get; private set; }
        public IDateTimeAdapter DateTimeAdapter { get; private set; }

        public GenericRepository(DbContext dbContext, CurrentUser user, IDateTimeAdapter dateTimeAdapter)
        {
            _dbContext = dbContext;
            _user = user;
            DateTimeAdapter = dateTimeAdapter;

            Entity = _dbContext.Set<T>();
        }

        private IQueryable<T> LoadNavigationProperties(IQueryable<T> query, Expression<Func<T, object>> includeProperties = null)
        {
            if (includeProperties != null && includeProperties.Body is NewExpression)
            {
                var navigationNames = (includeProperties.Body as NewExpression).Members.Select(a => a.Name).ToArray();
                foreach (var n in navigationNames)
                {
                    query = query.Include(n);
                }
            }

            return query;
        }

        private IQueryable<T> LoadNavigationProperties(IQueryable<T> query, string includeProperties = null)
        {
            if (!string.IsNullOrEmpty(includeProperties))
            {
                string[] props = includeProperties.Split(",", StringSplitOptions.TrimEntries);
                foreach (var n in props)
                {
                    query = query.Include(n);
                }
            }

            return query;
        }

        public async Task<bool> DeleteAsync(T entity, bool isSoftDelete = true)
        {
            if (isSoftDelete)
            {
                entity.IsDeleted = true;
                entity.LastModifiedOn = DateTimeAdapter.Now;
                entity.LastModifiedBy = _user?.UserId;
            }
            else
            {
                _dbContext.Set<T>().Remove(entity);
            }

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter, bool ignoreFilters = false, Expression<Func<T, object>> includeProperties = null, string includePropertiesAsString = null)
        {
            var entity = includeProperties != null ? LoadNavigationProperties(Entity, includeProperties) : LoadNavigationProperties(Entity, includePropertiesAsString);
            if (ignoreFilters)
            {
                entity = entity.AsNoTracking().IgnoreQueryFilters();
            }

            return await entity.FirstOrDefaultAsync(filter);
        }

        public async Task<NewT> GetAsync<NewT>(Expression<Func<T, bool>> filter, Expression<Func<T, NewT>> select, bool ignoreFilters = false, Expression<Func<T, object>> includeProperties = null, string includePropertiesAsString = null)
        {
            var entity = includeProperties != null ? LoadNavigationProperties(Entity, includeProperties) : LoadNavigationProperties(Entity, includePropertiesAsString);
            if (ignoreFilters)
            {
                entity = entity.AsNoTracking().IgnoreQueryFilters();
            }

            return await Task.FromResult(entity.Where(filter).Select(select).FirstOrDefault());
        }

        public async Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> filter, bool ignoreFilters = false, Expression<Func<T, object>> includeProperties = null, string includePropertiesAsString = null)
        {
            var entity = includeProperties != null ? LoadNavigationProperties(Entity, includeProperties) : LoadNavigationProperties(Entity, includePropertiesAsString);
            if (ignoreFilters)
            {
                entity.IgnoreQueryFilters();
            }

            return await Task.FromResult(entity.Where(filter).AsQueryable<T>());
        }

        public async Task<IQueryable<NewT>> GetAllAsync<NewT>(Expression<Func<T, bool>> filter, Expression<Func<T, NewT>> select, bool ignoreFilters = false, Expression<Func<T, object>> includeProperties = null, string includePropertiesAsString = null)
        {
            var entity = includeProperties != null ? LoadNavigationProperties(Entity, includeProperties) : LoadNavigationProperties(Entity, includePropertiesAsString);
            if (ignoreFilters)
            {
                entity.IgnoreQueryFilters();
            }

            return await Task.FromResult(entity.Where(filter).Select(select).AsQueryable<NewT>());
        }

        public async Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> filter, int pageNumber, int pageSize, bool ignoreFilters = false, Expression<Func<T, object>> includeProperties = null, string includePropertiesAsString = null)
        {
            var entity = includeProperties != null ? LoadNavigationProperties(Entity, includeProperties) : LoadNavigationProperties(Entity, includePropertiesAsString);
            if (ignoreFilters)
            {
                entity = entity.IgnoreQueryFilters();
            }

            var data = entity.Where(filter).OrderBy(a => a.CreatedOn).AsQueryable<T>();
            data = data
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsNoTracking()
                    .AsQueryable();

            return await Task.FromResult(data);
        }

        public async Task<T> GetByIdAsync(long entityId, bool ignoreFilters = false, Expression<Func<T, object>> includeProperties = null)
        {
            var entity = LoadNavigationProperties(Entity, includeProperties);
            if (ignoreFilters)
            {
                entity = entity.AsNoTracking().IgnoreQueryFilters();
            }

            return await entity.FirstOrDefaultAsync(p => p.Id == entityId);
        }

        public async Task<NewT> GetByIdAsync<NewT>(long entityId, Expression<Func<T, NewT>> select, bool ignoreFilters = false, Expression<Func<T, object>> includeProperties = null)
        {
            var entity = LoadNavigationProperties(Entity, includeProperties);
            if (ignoreFilters)
            {
                entity = entity.AsNoTracking().IgnoreQueryFilters();
            }

            return await Task.FromResult(entity.Where(p => p.Id == entityId).Select(select).FirstOrDefault());
        }

        public async Task<List<T>> GetListAsync(bool ignoreFilters = false)
        {
            var entity = Entity.AsQueryable<T>();
            if (ignoreFilters)
            {
                entity = entity.IgnoreQueryFilters();
            }

            return await entity.IgnoreQueryFilters().OrderBy(a => a.CreatedOn).ToListAsync();
        }

        public async Task<long> CountAsync(Expression<Func<T, bool>> filter, bool ignoreFilters = false)
        {
            var entity = Entity.AsQueryable<T>();
            if (ignoreFilters)
            {
                entity = entity.IgnoreQueryFilters();
            }

            return await entity.CountAsync(filter);
        }

        public async Task<bool> ExistAsync(Expression<Func<T, bool>> filter, bool ignoreFilters = false)
        {
            var entity = Entity.AsQueryable<T>();
            if (ignoreFilters)
            {
                entity = entity.IgnoreQueryFilters();
            }

            return (await entity.CountAsync(filter)) > 0;
        }

        public async Task<long> AddAsync(T entity)
        {
            entity.CreatedBy ??= _user?.UserId;
            entity.CreatedOn = DateTimeAdapter.Now;
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<long> AddRangeAsync(List<T> entities)
        {
            entities.ForEach(a =>
            {
                a.CreatedBy ??= _user?.UserId;
                a.CreatedOn = DateTimeAdapter.Now;
            });
            await _dbContext.AddRangeAsync(entities);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            entity.LastModifiedBy = _user?.UserId;
            entity.LastModifiedOn = DateTimeAdapter.Now;
            _dbContext.Update(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateRangeAsync(List<T> entities)
        {
            entities.ForEach(a =>
            {
                a.LastModifiedBy = _user?.UserId;
                a.LastModifiedOn = DateTimeAdapter.Now;
            });
            _dbContext.UpdateRange(entities);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<int> SqlQuery(string query)
        {
            if (query.ToLower().StartsWith("drop") || query.ToLower().StartsWith("alter")
                || query.ToLower().StartsWith("truncate") || query.ToLower().StartsWith("create"))
            {
                throw new ArgumentException("DDL commands not allowed");
            }

            return await _dbContext.Database.ExecuteSqlRawAsync(query);
        }
    }
}