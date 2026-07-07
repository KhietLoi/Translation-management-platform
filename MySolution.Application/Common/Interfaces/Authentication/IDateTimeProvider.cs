namespace MySolution.Application.Common.Interfaces;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
    public DateTime LocalNow { get; }
}