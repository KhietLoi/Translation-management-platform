using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class UserRoleRepository(AppDbContext context, ILogger logger) : Repository<UserRole>(context, logger), IUserRoleRepository
{
}