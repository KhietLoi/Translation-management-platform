using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class ProjectNamespaceRepository (AppDbContext context, ILogger logger)
    : Repository<ProjectNamespace>(context, logger), IProjectNamespaceRepository
{
    
}