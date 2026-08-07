using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class TranslationJobRepository (AppDbContext context, ILogger logger) :
    Repository<TranslationJob>(context, logger), ITranslationJobRepository
{
   
    public async Task<TranslationJob?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }
}