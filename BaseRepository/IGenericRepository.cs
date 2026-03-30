using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Boulevard.BaseRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> Add(T entity);
        T Addd(T entity);
        Task<T> Edit(T entityToUpdate);
        IQueryable<T> Get();
        IQueryable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "", bool isTrackingOff = false);
        IQueryable<T> Get(out int total, out int totalDisplay, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "", int pageIndex = 1, int pageSize = 10, bool isTrackingOff = false);
        Task<T> GetById(int id);
        Task<T> GetById(Int64 id);
        T GetbyId(int id);
        Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate);
        T GetFirstOrDefault(Expression<Func<T, bool>> predicate);
        int GetCount(Expression<Func<T, bool>> filter = null);
        void Remove(Expression<Func<T, bool>> filter);
        int Delete(IEnumerable<T> entity);

        void Remove(int id);
        void Remove(T entityToDelete);
        void MultipleRemove(List<T> entityToDeleteList);
        IQueryable<T> GetIQueryable();
        bool IsExist(Expression<Func<T, bool>> predicate);
        bool IsExist(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeExpressions);
        IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeExpressions);
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeExpressions);
    }
}
