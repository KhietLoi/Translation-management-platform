using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class RoleRepository(AppDbContext context, ILogger logger) : Repository<Role>(context, logger), IRoleRepository
{
}