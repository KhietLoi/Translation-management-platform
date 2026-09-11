using FluentValidation;

namespace MySolution.Application.Features.User.UserProfile.Commands.UploadAvatar;

public class UploadAvatarValidator : AbstractValidator<UploadAvatarCommand>
{
    //Check type of data:
    private readonly string[] _allowedExtensions = { ".png", ".jpg", ".jpeg" };
    private const int Filesize = (5*1024*1024);  //fix
    
    public UploadAvatarValidator()
    {
        RuleFor(x => x.Payload.AvatarFile)
            .NotNull();
        RuleFor(x => x.Payload.AvatarFile.Length)
            .LessThanOrEqualTo(Filesize);
        RuleFor(x => x.Payload.AvatarFile.FileName)
            .Must(HaveValidFileExtension)
            .WithMessage("Invalid file extension!");
    }
    
    public bool HaveValidFileExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return _allowedExtensions.Contains(extension,  StringComparer.InvariantCultureIgnoreCase);
    }
}