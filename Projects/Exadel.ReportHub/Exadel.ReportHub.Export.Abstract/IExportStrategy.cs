using Exadel.ReportHub.Data.Abstract;
using Exadel.ReportHub.SDK.Enums;

namespace Exadel.ReportHub.Export.Abstract;

public interface IExportStrategy
{
    Task<bool> SatisfyAsync(ExportFormat format, CancellationToken cancellationToken);

    Task<Stream> ExportAsync<TModel>(TModel exportModel, CancellationToken cancellationToken)
        where TModel : BaseReport;

    Task<Stream> ExportAsync<TModel>(IEnumerable<TModel> exportModels, CancellationToken cancellationToken)
        where TModel : BaseReport;
}
