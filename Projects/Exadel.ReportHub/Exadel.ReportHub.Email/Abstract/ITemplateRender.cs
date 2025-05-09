namespace Exadel.ReportHub.Email.Abstract;

public interface ITemplateRender
{
    Task<string> RenderAsync(string templateName, object data, CancellationToken cancellationToken);
}
