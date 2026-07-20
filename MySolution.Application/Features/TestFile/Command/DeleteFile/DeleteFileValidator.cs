using FluentValidation;

namespace MySolution.Application.Features.TestFile.Command.DeleteFile;

public class DeleteFileValidator : AbstractValidator<DeleteFileCommand>
{
    public DeleteFileValidator()
    {
        RuleFor(x => x.Payload.FileName)
            .NotEmpty()
            .WithMessage("File name is required.");
    }
}