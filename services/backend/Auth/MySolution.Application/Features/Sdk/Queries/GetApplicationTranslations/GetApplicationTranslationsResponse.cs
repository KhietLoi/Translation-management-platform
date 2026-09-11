using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Sdk.Queries.GetApplicationTranslations;

public class GetApplicationTranslationsResponse : BaseResponse <GetApplicationTranslationsData>
{

}

public class GetApplicationTranslationsData
{
    public Guid ProjectId { get; set; }
    public int Version { get; set; }
    public string Language { get; set; } = string.Empty;
    public Dictionary<string, string> Translations { get; init; } = new();
}