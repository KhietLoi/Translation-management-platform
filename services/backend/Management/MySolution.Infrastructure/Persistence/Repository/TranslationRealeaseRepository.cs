using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class TranslationReleaseRepository (AppDbContext context, ILogger logger) : Repository<TranslationRelease>(context, logger), ITranslationReleaseRepository
{
}   