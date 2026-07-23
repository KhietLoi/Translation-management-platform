using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Infrastructure.Persistence.Repository;

/// <summary>
///     Generic repository implementation for performing CRUD operations on entities of type T.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;
    protected readonly ILogger Logger;

    //Constructor:
    protected Repository(AppDbContext context, ILogger logger)
    {
        Context = context;
        Logger = logger;
        DbSet = Context.Set<T>();
    }

    public virtual IQueryable<T> GetAll()
    {
        return DbSet;
    }

    public virtual async Task<bool> Add(T entity)
    {
        await DbSet.AddAsync(entity);
        return true;
    }

    public virtual async Task<bool> AddRange(List<T> entity)
    {
        await DbSet.AddRangeAsync(entity);
        return true;
    }

    public virtual bool Delete(T entity)
    {
        DbSet.Remove(entity);
        return true;
    }

    public virtual bool DeleteRange(List<T> entities)
    {
        DbSet.RemoveRange(entities);
        return true;
    }

    public virtual async Task DeleteRangeAsync(Expression<Func<T, bool>> expression)
    {
        var queryable = DbSet.Where(expression);
        DbSet.RemoveRange(queryable);
        await Context.SaveChangesAsync();
    }

    public virtual IQueryable<T> Where(Expression<Func<T, bool>> expression)
    {
        return DbSet.Where(expression);
    }

    public EntityEntry<T> Update(T entity)
    {
        return DbSet.Update(entity);
    }
}