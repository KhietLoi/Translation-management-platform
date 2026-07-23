using FluentValidation;
using MySolution.Application.Features.TestFile.Command.DeleteFile;

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