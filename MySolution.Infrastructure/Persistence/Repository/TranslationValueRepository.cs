using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class TranslationValueRepository (AppDbContext context, ILogger logger) : 
    Repository<TranslationValue>(context,logger), ITranslationValueRepository
{
    
}