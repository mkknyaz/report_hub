using System.Net;
using System.Net.Mail;
using Exadel.ReportHub.Email.Abstract;
using Exadel.ReportHub.Email.Configs;
using Microsoft.Extensions.Options;

namespace Exadel.ReportHub.Email;

public class EmailSender(IOptionsMonitor<SmtpConfig> smtpConfig, ITemplateRender templateRender) : IEmailSender
{
    public async Task SendAsync(string to, string subject, IEnumerable<Attachment> attachments, string templateName, object data, CancellationToken cancellationToken)
    {
        var htmlBody = await templateRender.RenderAsync(templateName, data, cancellationToken);
        using var message = new MailMessage
        {
            From = new MailAddress(smtpConfig.CurrentValue.Email, smtpConfig.CurrentValue.DisplayName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(to);
        foreach (var attachment in attachments)
        {
            message.Attachments.Add(attachment);
        }

        using var smtpClient = new SmtpClient(smtpConfig.CurrentValue.Host, smtpConfig.CurrentValue.Port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(smtpConfig.CurrentValue.Email, smtpConfig.CurrentValue.Password)
        };

        await smtpClient.SendMailAsync(message, cancellationToken);
    }
}
