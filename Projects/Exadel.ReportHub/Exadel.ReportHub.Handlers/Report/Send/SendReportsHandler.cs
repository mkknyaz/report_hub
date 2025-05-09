using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using AutoMapper;
using Exadel.ReportHub.Email;
using Exadel.ReportHub.Email.Abstract;
using Exadel.ReportHub.Email.Models;
using Exadel.ReportHub.Pdf;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Exadel.ReportHub.Handlers.Report.Send;

public record SendReportsRequest : IRequest<Unit>;

public class SendReportsHandler(IUserRepository userRepository, IEmailSender emailSender, ILogger<SendReportsHandler> logger) : IRequestHandler<SendReportsRequest, Unit>
{
    public async Task<Unit> Handle(SendReportsRequest request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var currentHour = now.Hour;
        var currentDay = now.Day;
        var currentDayOfWeek = now.DayOfWeek;
        var users = await userRepository.GetUsersByNotificationSettingsAsync(currentDay, currentDayOfWeek, currentHour, cancellationToken);

        await Task.WhenAll(users.Select(async user =>
        {
            try
            {
                // var stream = generator
                // var attachment = new Attachment(stream, $"{reportName}.{user.NotificationSettings.ExportFormat}",);

                var reportEmail = new ReportEmailModel
                {
                    UserName = user.FullName,
                    StartDate = FormatDate(DateTime.Now), // Here'll be a report date
                    EndDate = FormatDate(DateTime.Now.AddDays(2)), // Here'll be a report date
                    IsSuccess = true // It'll be depend on generation status
                };

                await emailSender.SendAsync(user.Email, "Report", null, "Report.html", reportEmail, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send report email to user {UserId} ({Email})", user.Id, user.Email);
            }
        }));

        return Unit.Value;
    }

    private static string FormatDate(DateTime date) => date.ToString("dd.MM.yyyy");
}
