using Exadel.ReportHub.Host.Jobs.Abstract;
using Exadel.ReportHub.Host.Services;
using Hangfire;

namespace Exadel.ReportHub.Host.Jobs;

public class ExchangeRateJob : IJob
{
    public void Schedule()
    {
        RecurringJob.AddOrUpdate<ExchangeRateService>(
            recurringJobId: "ExchangeRateUpdater",
            methodCall: s => s.UpdateExchangeRatesAsync(),
            cronExpression: $"2 {Exadel.ReportHub.Ecb.Constants.UpdateHour} * * 1-5",
            options: new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });
    }
}
