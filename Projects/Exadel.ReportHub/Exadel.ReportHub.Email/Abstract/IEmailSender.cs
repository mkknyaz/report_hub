using System.Net.Mail;

namespace Exadel.ReportHub.Email.Abstract;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, IEnumerable<Attachment> attachments,  string templateName, object data, CancellationToken cancellationToken);
}
