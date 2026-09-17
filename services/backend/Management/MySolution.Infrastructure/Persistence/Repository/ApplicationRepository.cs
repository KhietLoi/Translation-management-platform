using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Infrastructure.Persistence.Repository;

public class ApplicationRepository (AppDbContext context, ILogger logger) : Repository<Domain.Entities.Application>(context, logger), IApplicationRepository
{
}