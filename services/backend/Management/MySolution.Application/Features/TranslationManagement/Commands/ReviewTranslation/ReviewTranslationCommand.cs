using MediatR;

namespace MySolution.Application.Features.TranslationManagement.Commands.ReviewTranslation;

public class ReviewTranslationCommand : IRequest<ReviewTranslationResponse>
{
   public Guid Id { get; set; }
   public ReviewTranslationCommand (Guid id)
   {
       Id = id;
   }
}