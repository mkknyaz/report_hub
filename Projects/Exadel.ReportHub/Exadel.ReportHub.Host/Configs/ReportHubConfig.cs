namespace Exadel.ReportHub.Host.Configs;

public class ReportHubConfig
{
    public Uri Host { get; init; }

    public TimeSpan ConnectionTimeout { get; set; }
}
