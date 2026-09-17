using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class TranslationKeyRepository (AppDbContext context, ILogger logger) : Repository<TranslationKey>(context, logger), ITranslationKeyRepository
{
}