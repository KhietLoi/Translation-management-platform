using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MySolution.Application.Common.Interfaces.Repositories;

/// <summary>
/// Interface Repository common method
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRepository<T> where T : class
{
    IQueryable<T>  GetAll();
    Task<bool> Add(T entity);
    
    Task<bool>AddRange(List<T> entity);
    bool Delete(T entity);
    bool DeleteRange(List<T> entities);
    
    Task DeleteRangeAsync(Expression<Func<T, bool>> expression);
    IQueryable<T> Where (Expression<Func<T, bool>> expression);
    
    EntityEntry<T> Update(T entity);
    
    
    
}