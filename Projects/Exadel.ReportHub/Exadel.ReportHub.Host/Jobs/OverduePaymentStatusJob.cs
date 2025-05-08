using Exadel.ReportHub.Host.Jobs.Abstract;
using Exadel.ReportHub.SDK.Abstract;
using Hangfire;

namespace Exadel.ReportHub.Host.Jobs;

public class OverduePaymentStatusJob : IJob
{
    public void Schedule()
    {
        RecurringJob.AddOrUpdate<IInvoiceService>(
            recurringJobId: Constants.Job.Id.OverduePaymentStatusUpdater,
            methodCall: s => s.UpdateOverdueInvoicesStatusAsync(),
            cronExpression: "1 0 * * *",
            options: new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });
    }
}
