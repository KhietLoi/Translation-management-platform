using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Infrastructure.Persistence.Repository;

public class Repository<T> : IRepository<T> where T: class
{
    
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger _logger;
    
    //Constructor:
    protected Repository(AppDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
        _dbSet = _context.Set<T>();
    }
    public virtual IQueryable<T> GetAll()
    {
        return _dbSet;
    }

    public virtual async Task<bool> Add(T entity)
    {
        await _dbSet.AddAsync(entity);
        return true;
    }

    public virtual async Task<bool> AddRange(List<T> entity)
    {
        await _dbSet.AddRangeAsync(entity);
        return true;
    }

    public virtual bool Delete(T entity)
    {
        _dbSet.Remove(entity);
        return true;
    }

    public virtual bool DeleteRange(List<T> entities)
    {
        _dbSet.RemoveRange(entities);
        return true;
    }

    public virtual async Task DeleteRangeAsync(Expression<Func<T, bool>> expression)
    {
        var queryable = _dbSet.Where(expression);
        _dbSet.RemoveRange(queryable);
        await _context.SaveChangesAsync();
    }

    public virtual IQueryable<T> Where(Expression<Func<T, bool>> expression)
    {
        return _dbSet.Where(expression);
    }

    public EntityEntry<T> Update(T entity)
    {
        return _dbSet.Update(entity);
    }

    
}