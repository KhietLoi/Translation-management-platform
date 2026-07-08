using MySolution.Application.Common.Interfaces;

namespace MySolution.Infrastructure.Authentication;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow  => DateTime.UtcNow;
    public DateTime LocalNow => DateTime.Now;
}