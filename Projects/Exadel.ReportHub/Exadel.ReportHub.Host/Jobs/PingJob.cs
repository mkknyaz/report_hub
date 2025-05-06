using Exadel.ReportHub.Host.Jobs.Abstract;
using Hangfire;

namespace Exadel.ReportHub.Host.Jobs;

public class PingJob(IHttpClientFactory clientFactory, ILogger<PingJob> logger) : IJob
{
    public void Schedule()
    {
        RecurringJob.AddOrUpdate<PingJob>(
            recurringJobId: Constants.Job.Id.Ping,
            methodCall: j => j.PingAsync(),
            cronExpression: "*/14 * * * *",
            options: new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });
    }

    public async Task PingAsync()
    {
        try
        {
            var client = clientFactory.CreateClient(Constants.HttpClient.PingClient);
            var response = await client.GetAsync(Constants.HttpClient.Path.Ping);
            response.EnsureSuccessStatusCode();

            logger.LogInformation("Ping Job – BaseAddress: {BaseAddress}, StatusCode: {StatusCode}", client.BaseAddress, response.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ping failed");
        }
    }
}
