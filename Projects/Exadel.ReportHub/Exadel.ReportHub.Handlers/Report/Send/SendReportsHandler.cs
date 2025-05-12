using System.Net.Mail;
using Exadel.ReportHub.Email.Abstract;
using Exadel.ReportHub.Email.Models;
using Exadel.ReportHub.Handlers.Managers.Report;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Report;
using Exadel.ReportHub.SDK.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Exadel.ReportHub.Handlers.Report.Send;

public record SendReportsRequest : IRequest<Unit>;

public class SendReportsHandler(IReportManager reportManager, IUserRepository userRepository, IEmailSender emailSender, ILogger<SendReportsHandler> logger)
    : IRequestHandler<SendReportsRequest, Unit>
{
    public async Task<Unit> Handle(SendReportsRequest request, CancellationToken cancellationToken)
    {
        const int daysInWeek = 7;
        const string subject = "Report";

        var now = DateTime.UtcNow;
        var currentHour = now.Hour;
        var currentDay = now.Day;
        var currentDayOfWeek = now.DayOfWeek;
        var today = DateTime.Today;
        var users = await userRepository.GetUsersByNotificationSettingsAsync(currentDay, currentDayOfWeek, currentHour, cancellationToken);

        await Task.WhenAll(users.Select(async user =>
        {
            var (startDate, endDate) = user.NotificationSettings.ReportPeriod switch
            {
                Data.Enums.ReportPeriod.Whole => (null, null),
                Data.Enums.ReportPeriod.Month => (today.AddMonths(-1).AddDays(1), today),
                Data.Enums.ReportPeriod.Week => (today.AddDays(-daysInWeek), today),
                Data.Enums.ReportPeriod.Custom => (today.AddDays(-(user.NotificationSettings.DaysCount!.Value - 1)), today),
                _ => ((DateTime?)null, (DateTime?)null)
            };

            var exportReportDto = new ExportReportDTO
            {
                ClientId = user.NotificationSettings.ClientId,
                Format = (ExportFormat)user.NotificationSettings.ExportFormat,
                StartDate = startDate,
                EndDate = endDate
            };

            var reportEmail = new EmailReportData
            {
                UserName = user.FullName,
                ClientName = user.NotificationSettings.ClientName,
                Period = user.NotificationSettings.ReportPeriod is Data.Enums.ReportPeriod.Whole ?
                    "whole period" :
                    $"{FormatDate(exportReportDto.StartDate.Value)} to {FormatDate(exportReportDto.EndDate.Value)}"
            };

            var attachments = new List<Attachment>();
            try
            {
                var invoicesReportTask = reportManager.GenerateInvoicesReportAsync(exportReportDto, cancellationToken);
                var itemsReportTask = reportManager.GenerateItemsReportAsync(exportReportDto, cancellationToken);
                var plansReportTask = reportManager.GeneratePlansReportAsync(exportReportDto, cancellationToken);

                await Task.WhenAll(invoicesReportTask, itemsReportTask, plansReportTask);
                attachments.Add(new Attachment(invoicesReportTask.Result.Stream, invoicesReportTask.Result.FileName));
                attachments.Add(new Attachment(itemsReportTask.Result.Stream, itemsReportTask.Result.FileName));
                attachments.Add(new Attachment(plansReportTask.Result.Stream, plansReportTask.Result.FileName));
                reportEmail.IsSuccess = true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to generate reports");
                reportEmail.IsSuccess = false;
            }

            try
            {
                await emailSender.SendAsync(user.Email, subject, attachments, Email.Constants.ResourcePath.TemplateName, reportEmail, cancellationToken);
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
