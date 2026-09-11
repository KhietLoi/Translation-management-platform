using MySolution.Application.Common.Models;

namespace MySolution.Application.Common.Interfaces.Authentication;

public interface IApiKeyContextAccessor
{
    ApiKeyContext? Current { get; }
}