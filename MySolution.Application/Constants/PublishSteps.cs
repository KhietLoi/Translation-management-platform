namespace MySolution.Application.Constants;

public static class PublishSteps
{
    public const int Validate = 1;
    public const int GenerateJson = 2;
    public const int UploadBlob = 3;
    public const int CreateRelease = 4;
    public const int Complete = 5;
}