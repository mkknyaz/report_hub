using System.Net.Mail;
using Exadel.ReportHub.Email.Models;

namespace Exadel.ReportHub.Email.Abstract;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, Attachment attachment,  string templateName, object data, CancellationToken cancellationToken);
}
