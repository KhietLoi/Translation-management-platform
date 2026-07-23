using MySolution.Application.Common.Interfaces;
using Scriban;

namespace MySolution.Application.Service.Scriban;

public class ScribanTemplateRenderer : ITemplateRenderer
{
    public async Task<string> RenderAsync(
        string templateName,
        object model,
        CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Common",
            "Template",
            templateName);

        var content = await File.ReadAllTextAsync(path, cancellationToken);

        var template = Template.Parse(content);

        if (template.HasErrors) throw new Exception(string.Join(Environment.NewLine, template.Messages));

        return template.Render(model);
    }
}