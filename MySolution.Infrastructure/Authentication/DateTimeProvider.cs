using MySolution.Application.Common.Interfaces;

namespace MySolution.Infrastructure.Authentication;

/// <summary>
/// Provides the current date and time.
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow  => DateTime.UtcNow;
    public DateTime LocalNow => DateTime.Now;
}