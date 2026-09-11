namespace MySolution.Application.Common.Interfaces.Authentication;

public interface ISecurityStampService
{
    Task SetSecurityStampAsync(Guid userId, string securityStamp);
    Task<string?> GetSecurityStampAsync(Guid userId);
}