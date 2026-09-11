using MySolution.Email.Application.Common.Interfaces;
using Scriban;

namespace MySolution.Email.Infrastructure.Services;

public class ScribanTemplateRenderer : ITemplateRenderer
{
    public async Task<string> RenderAsync(
        string templateName,
        object model,
        CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Templates",
            templateName);

        Console.WriteLine(AppContext.BaseDirectory);
        Console.WriteLine(path);
        Console.WriteLine(File.Exists(path));
        var content = await File.ReadAllTextAsync(path, cancellationToken);
        var template = Template.Parse(content);
        if (template.HasErrors) throw new Exception(string.Join(Environment.NewLine, template.Messages));

        return await template.RenderAsync(model);
    }
}