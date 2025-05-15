using Exadel.ReportHub.Data.Abstract;
using Exadel.ReportHub.Export.Abstract.Models;
using Exadel.ReportHub.SDK.Enums;

namespace Exadel.ReportHub.Export.Abstract;

public interface IExportStrategy
{
    Task<bool> SatisfyAsync(ExportFormat format, CancellationToken cancellationToken);

    Task<Stream> ExportAsync<TModel>(TModel exportModel, ChartData chartData = null, CancellationToken cancellationToken = default)
        where TModel : BaseReport;

    Task<Stream> ExportAsync<TModel>(IEnumerable<TModel> exportModels, ChartData chartData = null, CancellationToken cancellationToken = default)
        where TModel : BaseReport;
}
