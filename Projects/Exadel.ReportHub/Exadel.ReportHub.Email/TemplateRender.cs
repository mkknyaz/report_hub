using Exadel.ReportHub.Email.Abstract;
using Stubble.Core;
using Stubble.Core.Builders;
using Stubble.Core.Interfaces;

namespace Exadel.ReportHub.Email;

public class TemplateRender : ITemplateRender
{
    public async Task<string> RenderAsync(string templateName, object data, CancellationToken cancellationToken)
    {
        var path = Path.Combine(Constants.ResourcePath.EmailTemplates, templateName);
        var text = await File.ReadAllTextAsync(path, cancellationToken);
        return await new StubbleBuilder().Build().RenderAsync(text, data);
    }
}
