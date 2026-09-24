using MediatR;

namespace MySolution.Application.Features.Sdk.Queries.GetApplicationTranslations;

public class GetApplicationTranslationsQuery : IRequest<GetApplicationTranslationsResponse>
{
   public string Language { get; set; }
   public GetApplicationTranslationsQuery(string language)
   {
      Language = language;
   }
}