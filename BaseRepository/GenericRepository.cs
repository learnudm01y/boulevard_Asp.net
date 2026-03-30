using Boulevard.Contexts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace Boulevard.BaseRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public BoulevardDbContext _dbContext;
        public DbSet<T> _dbSet;

        public GenericRepository(BoulevardDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = this._dbContext.Set<T>();
        }

        public async Task<T> Add(T entity)
        {
            _dbSet.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public T Addd(T entity)
        {
            _dbSet.Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public virtual int Delete(IEnumerable<T> entity)
        {
            _dbContext.Set<T>().RemoveRange(entity);
            return _dbContext.SaveChanges();
        }
        public IQueryable<T> GetIQueryable()
        {
            return _dbSet;
        }

        public async Task<T> Edit(T entityToUpdate)
        {
            _dbSet.Add(entityToUpdate);
            _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return entityToUpdate;
        }

        public IQueryable<T> Get()
        {
            return _dbSet.AsQueryable();
        }
        public virtual bool IsExist(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().Any(predicate);
        }
        public virtual bool IsExist(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeExpressions)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeExpressions.Aggregate(query, (current, inc) => current.Include(inc));
            return query.Any(predicate);
        }
        public IQueryable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "", bool isTrackingOff = false)
        {
            var query = _dbSet.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
               (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            IQueryable<T> result = null;

            if (orderBy != null)
            {
                result = orderBy(query);
            }

            if (isTrackingOff)
                return result?.AsNoTracking();
            else
                return result;
        }
        public virtual async Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
        }
        public virtual T GetFirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().FirstOrDefault(predicate);
        }
        public IQueryable<T> Get(out int total, out int totalDisplay, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "", int pageIndex = 1, int pageSize = 10, bool isTrackingOff = false)
        {
            var query = _dbSet.AsQueryable();
            total = query.Count();
            totalDisplay = total;

            if (filter != null)
            {
                query = query.Where(filter);
                totalDisplay = query.Count();
            }

            foreach (var includeProperty in includeProperties.Split
               (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            IQueryable<T> result = null;

            if (orderBy != null)
            {
                result = orderBy(query).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            else
            {
                result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }

            if (isTrackingOff)
                return result?.AsNoTracking();
            else
                return result;
        }

        public async Task<T> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> GetById(Int64 id)
        {
            return await _dbSet.FindAsync(id);
        }

        public T GetbyId(int id)
        {
            return _dbSet.Find(id);
        }
        public int GetCount(Expression<Func<T, bool>> filter = null)
        {
            var query = _dbSet.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.Count();
        }

        public void Remove(Expression<Func<T, bool>> filter)
        {
            _dbSet.RemoveRange(_dbSet.Where(filter));
        }

        public void Remove(int id)
        {
            var entityToDelete = _dbSet.Find(id);
            Remove(entityToDelete);
        }

        public void Remove(T entityToDelete)
        {
            if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
            _dbContext.SaveChanges();
        }


        public void MultipleRemove(List<T> entityToDeleteList)
        {
            foreach (var entityToDelete in entityToDeleteList)
            {
                if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
                {
                    _dbSet.Attach(entityToDelete);
                }
                _dbSet.Remove(entityToDelete);
                _dbContext.SaveChanges();
            }
        }
        public virtual IQueryable<T> GetAll(params Expression<Func<T, object>>[] includeExpressions)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeExpressions.Aggregate(query, (current, inc) => current.Include(inc));
            return query.AsQueryable();
        }
        public virtual IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeExpressions)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeExpressions.Aggregate(query, (current, inc) => current.Include(inc));
            return query.Where(predicate).AsQueryable();
        }
    }
}