using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Server.Model
{
    public class GenericRepository<T> : IGenericRepository<T> where T : EntityBase
    {
        protected readonly DbContext _dbContext;
        public readonly DbSet<T> _dbSet;

        protected GenericRepository(DbContext dbContext)
        {
            _dbSet = dbContext.Set<T>();
            _dbContext = dbContext;
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, bool isActive = true)
        {
            return await _dbSet.Where(predicate).Where(s => s.IsActive == isActive).CountAsync();
        }

        public void Delete(T model)
        {
            model.IsActive = false;
            model.ModifyDateTime = DateTime.Now;
            _dbSet.Update(model);
        }

        public async Task DeleteListAsync(List<T> list)
        {
            list.ForEach(x => { x.IsActive = false; x.ModifyDateTime = DateTime.Now; });
            await _dbContext.BulkUpdateAsync(list);
        }

        public async Task DeleteListPermanentlyAsync(List<T> list)
        {
            await _dbContext.BulkDeleteAsync(list);
        }

        public void DeletePermanently(T model)
        {
            _dbSet.Remove(model);
        }

        public async Task<List<T>> FilterByAsync(Expression<Func<T, bool>> predicate, bool isActive = true)
        {
            return await _dbSet.Where(predicate).Where(s => s.IsActive == isActive).ToListAsync();
        }

        public async Task<T> FindByAsync(Expression<Func<T, bool>> predicate, bool isActive = true)
        {
            return await _dbSet.Where(predicate).SingleOrDefaultAsync(m => isActive);
        }

        public async Task<List<T>> GetAllAsync(bool isActive = true)
        {
            return await _dbSet.Where(m => m.IsActive == isActive).ToListAsync();
        }

        public async Task<List<T>> FilterWithPagingAsync(Expression<Func<T, bool>> predicate = null, int index = 0, int size = 10, bool isActive = true)
        {
            return await _dbSet.Where(predicate).Where(m => m.IsActive == isActive).Skip(index * size).Take(size).ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id, bool isActive = true)
        {
            return await _dbSet.SingleOrDefaultAsync(m => m.Id == id && m.IsActive == isActive);
        }

        public async Task<T> InsertAsync(T model)
        {
            var entityEntry = await _dbSet.AddAsync(model);
            return entityEntry.Entity;
        }

        public async Task<List<T>> InsertListAsync(List<T> list)
        {
            foreach (EntityBase enty in list)
            {
                enty.CreateDateTime = DateTime.Now;
                enty.ModifyDateTime = null;
                enty.IsActive = true;
            }
            await _dbContext.BulkInsertAsync(list);
            return list;
        }

        public T Update(T model)
        {
            var entityEntry = _dbSet.Update(model);
            return entityEntry.Entity;
        }

        public async Task<List<T>> UpdateListAsync(List<T> list)
        {
            list.ForEach(x => { x.ModifyDateTime = DateTime.Now; });
            await _dbContext.BulkUpdateAsync(list);
            return list;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, bool isActive = true)
        {
            return await _dbSet.Where(s => s.IsActive == isActive).AnyAsync(predicate);
        }
    }
}
