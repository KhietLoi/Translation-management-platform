using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class OrganizationMemberRepository (AppDbContext context, ILogger logger) : Repository<OrganizationMember>(context, logger), IOrganizationMemberRepository
{
    
}