using FluentValidation;

namespace MySolution.Application.Features.Admin.TestFile.Command.DeleteFile;

public class DeleteFileValidator : AbstractValidator<DeleteFileCommand>
{
    public DeleteFileValidator()
    {
        RuleFor(x => x.Payload.FileName)
            .NotEmpty()
            .WithMessage("File name is required.");
    }
}