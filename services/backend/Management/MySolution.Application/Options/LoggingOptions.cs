namespace MySolution.Application.Options;

public class LoggingOptions
{
    public static readonly string OptionName = "Logging";
    public bool ConsoleEnabled { get; set; }
    public Seq? Seq { get; set; } = default;
    public Elk? Elk { get; set; } = default;
    public MicrosoftTeams? MicrosoftTeams { get; set; } = default;
}

public class Seq
{
    public bool Enabled { get; set; }
    public string Url { get; set; } = default!;
    public string ApiKey { get; set; } = default!;
}

public class Elk
{
    public bool Enabled { get; set; }
    // public string ElasticSearchUrl { get; set; } = default!;
    public string ElasticSearchUrl { get; set; } = "http://localhost:9200";
    public string DataStream { get; set; } = "translation.management";
}

public class MicrosoftTeams
{
    public bool Enabled { get; set; } = false;
    public string WebHookUri { get; set; } = default!;
    public bool EnabledCriticalLevel { get; set; } = false;
    public string WebHookUriCriticalLevel { get; set; } = default!;
    public int BatchSizeLimit { get; set; }
    public int Period { get; set; }
}