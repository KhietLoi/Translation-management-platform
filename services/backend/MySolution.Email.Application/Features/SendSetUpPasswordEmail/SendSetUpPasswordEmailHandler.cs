    using MediatR;
    using MySolution.Email.Application.Common.Bases;
    using MySolution.Email.Application.Common.Enums;
    using MySolution.Email.Application.Common.Interfaces;
    using MySolution.Email.Application.Common.Models;


    namespace MySolution.Email.Application.Features.SendSetUpPasswordEmail;

    public class SendSetUpPasswordEmailHandler
        : BaseEmailHandler,
            IRequestHandler<SendSetUpPasswordEmailCommand>
    {
        private readonly ITokenSetting _tokenSettings;
        private readonly IEmailTemplateFactory<StandardEmailTemplateData, EmailTemplateModel> _emailTemplateFactory;

        public SendSetUpPasswordEmailHandler(
            IApplicationUrlProvider urlProvider,
            IEmailTemplateService emailTemplateService,
            ITokenSetting tokenSetting,
            IEmailTemplateFactory<StandardEmailTemplateData, EmailTemplateModel> emailTemplateFactory
            ) : base(urlProvider, emailTemplateService)
        {
            _tokenSettings = tokenSetting;
            _emailTemplateFactory = emailTemplateFactory;
        }

        public Task Handle(SendSetUpPasswordEmailCommand request, CancellationToken cancellationToken)
        {
            var data = new StandardEmailTemplateData
            {
                UserName = request.Message.Username,
                ActionUrl = UrlProvider.GetResetPasswordUrl(request.Message.Token),
                ExpiryMinutes = _tokenSettings.PasswordResetExpiryMinutes
            };
            var model = _emailTemplateFactory.Create(data);
            
            return SendTemplateAsync(EmailType.SetUpPassword,request.Message.Email, model,  cancellationToken);
        }
    }