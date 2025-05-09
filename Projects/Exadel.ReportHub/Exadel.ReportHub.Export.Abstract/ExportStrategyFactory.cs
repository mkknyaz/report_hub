using Exadel.ReportHub.SDK.Enums;

namespace Exadel.ReportHub.Export.Abstract;

public class ExportStrategyFactory(IEnumerable<IExportStrategy> strategies) : IExportStrategyFactory
{
    public async Task<IExportStrategy> GetStrategyAsync(ExportFormat format, CancellationToken cancellationToken)
    {
        foreach (var strategy in strategies)
        {
            if (await strategy.SatisfyAsync(format, cancellationToken))
            {
                return strategy;
            }
        }

        throw new ArgumentException($"Unsupported export format: {format}");
    }
}
