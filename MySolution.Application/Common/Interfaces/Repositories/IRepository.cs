using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MySolution.Application.Common.Interfaces.Repositories;
/// <summary>
/// Defines a generic repository interface for performing CRUD operations on entities of type T.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets all entities of type T as an IQueryable for further querying.
    /// </summary>
    /// <returns></returns>
    IQueryable<T>  GetAll();
    /// <summary>
    /// Adds a new entity of type T to the repository asynchronously.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<bool> Add(T entity);
    /// <summary>
    /// Adds a range of entities of type T to the repository asynchronously.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<bool>AddRange(List<T> entity);
    /// <summary>
    /// Deletes an entity of type T from the repository.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    bool Delete(T entity);
    /// <summary>
    /// Deletes a range of entities of type T from the repository.
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    bool DeleteRange(List<T> entities);
    /// <summary>
    /// Deletes a range of entities of type T from the repository asynchronously.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    Task DeleteRangeAsync(Expression<Func<T, bool>> expression);
    /// <summary>
    /// Filters entities of type T based on a given expression.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    IQueryable<T> Where (Expression<Func<T, bool>> expression);
    /// <summary>
    /// Updates an entity of type T in the repository.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    EntityEntry<T> Update(T entity);
}