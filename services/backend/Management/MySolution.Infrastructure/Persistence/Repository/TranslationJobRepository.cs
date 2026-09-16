using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;

namespace MySolution.Infrastructure.Persistence.Repository;

public class TranslationJobRepository (AppDbContext context, ILogger logger) :
    Repository<TranslationJob>(context, logger), ITranslationJobRepository
{
   
    public async Task<TranslationJob?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    // public async Task<(List<TranslationJob> Items, int TotalCount)> GetHistoryAsync(Guid projectId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    // {
    //     var query = DbSet
    //         .AsNoTracking()     
    //         .Where(x => x.ProjectId == projectId && 
    //                     (x.Type == TranslationJobType.Export || x.Type == TranslationJobType.Import ));
    //     var totalCount = await query.CountAsync(cancellationToken);
    //     var items = await query
    //         .OrderByDescending(x => x.CreatedAt)
    //         .Skip((pageNumber - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToListAsync(cancellationToken);
    //
    //     return (items, totalCount);
    // }
}