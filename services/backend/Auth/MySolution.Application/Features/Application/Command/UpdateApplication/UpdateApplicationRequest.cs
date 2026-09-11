namespace MySolution.Application.Features.Application.Command.UpdateApplication;

public class UpdateApplicationRequest
{ 
    public string Name { get; set; } = String.Empty;
    public string? Description { get; set; } 
    public bool IsActive { get; set; } 
}