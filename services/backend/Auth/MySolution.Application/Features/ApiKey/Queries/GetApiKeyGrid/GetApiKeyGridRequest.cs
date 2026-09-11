namespace MySolution.Application.Features.ApiKey.Queries.GetApiKeyGrid;

public class GetApiKeyGridRequest
{
    public Guid? ProjectId { get; set; }
    public Guid? ApplicationId { get; set; }
    public string? Keyword { get; set; }
    public bool? IsRevoked { get; set; }
    public int Page { get; set; } = 1;
    public int Limit { get; set; } = 20;
}