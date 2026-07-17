namespace MySolution.Application.Options;

public class AppOptions
{
    public static readonly string OptionName = "MySolution";
    public string Name { get; set; } =  "MySolution";
    public string HostingUrl { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public int TokenExpirationInMinutes { get; set; }
}