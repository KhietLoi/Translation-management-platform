using FluentValidation;

namespace MySolution.Application.Features.Auth.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        // Add validation rules here if needed
    }
}