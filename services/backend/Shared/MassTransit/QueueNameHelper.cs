using MassTransit;

namespace Shared.MassTransit;

public static class QueueNameHelper
{
    private static readonly KebabCaseEndpointNameFormatter _formatter = new(false);
    public static string Get<T>()
    {
        return _formatter.SanitizeName(typeof(T).Name);
    }
}