namespace MySolution.Application.Features.Application.Command.CreateApplication;

public class CreateApplicationRequest
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}