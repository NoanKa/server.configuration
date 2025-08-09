using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Server.Model
{
    public interface IGenericRepository<T> where T : EntityBase
    {
        Task<T> GetByIdAsync(Guid id, bool isActive = true);
        Task<List<T>> GetAllAsync(bool isActive = true);
        Task<List<T>> FilterWithPagingAsync(Expression<Func<T, bool>> predicate = null, int index = 0, int size = 10, bool isActive = true);
        Task<T> FindByAsync(Expression<Func<T, bool>> predicate, bool isActive = true);
        Task<List<T>> FilterByAsync(Expression<Func<T, bool>> predicate, bool isActive = true);
        Task<T> InsertAsync(T model);
        Task<List<T>> InsertListAsync(List<T> list);
        Task DeleteListAsync(List<T> list);
        Task DeleteListPermanentlyAsync(List<T> list);
        Task<List<T>> UpdateListAsync(List<T> list);
        T Update(T model);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate, bool isActive = true);
        void Delete(T model);
        void DeletePermanently(T model);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, bool isActive = true);
    }
}
