using Exadel.ReportHub.Host.Jobs.Abstract;
using Exadel.ReportHub.SDK.Abstract;
using Hangfire;

namespace Exadel.ReportHub.Host.Jobs;

public class ReportNotificationJob : IJob
{
    public void Schedule()
    {
        RecurringJob.AddOrUpdate<IReportService>(
            recurringJobId: Constants.Job.Id.SendUserReports,
            methodCall: j => j.SendReportsAsync(),
            cronExpression: "0 * * * *",
            options: new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });
    }
}
