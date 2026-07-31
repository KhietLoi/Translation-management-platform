namespace MySolution.Api.Options;

public class HttpRateLimitOptions
{
    public const string SectionName = "HttpRateLimit";
    public HttpRateLimitPolicy Login { get; set; } = new();
    public HttpRateLimitPolicy Register { get; set; } = new();
    public HttpRateLimitPolicy ForgotPassword { get; set; } = new();
    public HttpRateLimitPolicy RefreshToken { get; set; } = new();
    public class HttpRateLimitPolicy
    {
        public int PermitLimit { get; set; }
        public int WindowMinutes { get; set; }
        public int QueueLimit { get; set; }
    }
}