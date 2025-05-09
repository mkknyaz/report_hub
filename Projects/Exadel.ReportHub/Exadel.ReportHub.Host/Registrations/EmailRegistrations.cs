using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mail;
using Aspose.Pdf.Forms;
using Exadel.ReportHub.Email;
using Exadel.ReportHub.Email.Abstract;
using Exadel.ReportHub.Email.Configs;
using Microsoft.Extensions.Options;

namespace Exadel.ReportHub.Host.Registrations;

[ExcludeFromCodeCoverage]
public static class EmailRegistrations
{
    public static IServiceCollection AddEmailSender(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmtpConfig>(configuration.GetSection(nameof(SmtpConfig)));

        services.AddSingleton<ITemplateRender, TemplateRender>();
        services.AddSingleton<IEmailSender, EmailSender>();

        return services;
    }
}
