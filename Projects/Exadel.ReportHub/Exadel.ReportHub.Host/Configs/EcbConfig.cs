namespace Exadel.ReportHub.Host.Configs;

public class EcbConfig
{
    public Uri Host { get; init; }

    public TimeSpan ConnectionTimeout { get; set; }
}
