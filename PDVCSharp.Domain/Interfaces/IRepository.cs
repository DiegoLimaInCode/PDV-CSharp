using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PDVCSharp.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Returns all entities of type T (excluding soft-deleted).
        /// </summary>
        IQueryable<T> GetAll();

        /// <summary>
        /// Returns an entity of type T by its ID.
        /// </summary>
        /// <param name="id">Entity ID.</param>
        /// <returns>The corresponding entity or null if not found.</returns>
        Task<T?> GetById(Guid id);

        /// <summary>
        /// Adds a new entity of type T.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        Task<Guid> Add(T entity);

        /// <summary>
        /// Updates an existing entity of type T.
        /// </summary>
        /// <param name="entity">The entity with updated information.</param>
        Task Update(T entity);

        /// <summary>
        /// Deletes an entity of type T.
        /// </summary>
        /// <param name="id">ID of the entity to be deleted.</param>
        Task Delete(T entity);

        /// <summary>
        /// Saves changes in the database context.
        /// </summary>
        Task Commit();

        /// <summary>
        /// Returns a list of entities of type T that match a specific filter.
        /// </summary>
        /// <param name="predicate">Expression to filter the entities.</param>
        /// <returns>List of filtered entities.</returns>
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
    }
}
