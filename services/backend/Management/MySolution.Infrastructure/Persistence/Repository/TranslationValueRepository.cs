using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Models;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;

namespace MySolution.Infrastructure.Persistence.Repository;

public class TranslationValueRepository (AppDbContext context, ILogger logger) : Repository<TranslationValue>(context,logger), ITranslationValueRepository
{
}