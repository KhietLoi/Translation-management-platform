using MySolution.Email.Application.Common.Models;

namespace MySolution.Email.Application.Common.Interfaces;

/*public interface IEmailTemplateFactory : IEmailTemplateFactory <in TInput, out TModel>  where TInput : class
    where TModel : class
{
    EmailTemplateModel Create(string userName, string actionUrl, int expiryMinutes);
}*/
public interface IEmailTemplateFactory<in TInput, out TModel>
    where TInput : class
    where TModel : class
{
    TModel Create(TInput input);
}