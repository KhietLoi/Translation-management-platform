using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class OrganizationRepository (AppDbContext context, ILogger logger) : Repository<Organization>(context, logger), IOrganizationRepository
{
}