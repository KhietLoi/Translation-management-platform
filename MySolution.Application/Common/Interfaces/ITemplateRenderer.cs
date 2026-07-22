namespace MySolution.Application.Common.Interfaces;

public interface ITemplateRender
{
    Task<string> RenderAsync(string templateName, object model, CancellationToken cancellationToken = default);
}