using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PDVCSharp.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<T?> GetById(Guid id);
        Task<Guid> Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task Commit();
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
    }
}
