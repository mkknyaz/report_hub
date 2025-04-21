using Exadel.ReportHub.Host.Jobs.Abstract;
using Exadel.ReportHub.SDK.Abstract;

namespace Exadel.ReportHub.Host.Services;

public class SchedulerService(IEnumerable<IJob> jobs) : ISchedulerService
{
    public void StartJobs()
    {
        foreach (var job in jobs)
        {
            job.Schedule();
        }
    }
}
