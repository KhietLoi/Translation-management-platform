using MediatR;
using MySolution.Application.Features.TranslationValue.Queries.GetTranslationValues;
using MySolution.Domain.Enums;

namespace MySolution.Application.Features.TranslationManagement.Queries.GetTranslationValues;

public class GetTranslationValuesQuery : IRequest<GetTranslationValuesResponse>
{
   public Guid? TranslationKeyId { get; set; }
   public Guid? NamespaceId { get; set; }
   public Guid? LanguageId { get; set; }
   public TranslationStatus? Status { get; set; }

    public GetTranslationValuesQuery
    (
         Guid? translationKeyId,
         Guid? namespaceId,
         Guid? languageId,
         TranslationStatus? status
    )
    {
        TranslationKeyId = translationKeyId;
        NamespaceId = namespaceId;
        LanguageId = languageId;
        Status = status;
    }
}