using System.ComponentModel.DataAnnotations;

namespace MySolution.Infrastructure.Options;

public class RedisOptions
{
    public const string SectionName = "Redis";
    [Required]
    public string ConnectionString { get; set; } = string.Empty;
}