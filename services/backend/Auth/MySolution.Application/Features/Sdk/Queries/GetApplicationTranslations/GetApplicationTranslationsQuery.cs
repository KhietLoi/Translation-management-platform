using MediatR;

namespace MySolution.Application.Features.Sdk.Queries.GetApplicationTranslations;

public class GetApplicationTranslationsQuery : IRequest<GetApplicationTranslationsResponse>
{
   //public Guid ProjectId { get; set; }
   public string Language { get; set; } = string.Empty;
}