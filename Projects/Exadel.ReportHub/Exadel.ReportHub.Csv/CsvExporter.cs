using System.Globalization;
using CsvHelper;
using Exadel.ReportHub.Csv.ClassMaps;
using Exadel.ReportHub.Data.Abstract;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Export.Abstract.Models;
using Exadel.ReportHub.SDK.Enums;

namespace Exadel.ReportHub.Csv;

public class CsvExporter : IExportStrategy
{
    public Task<bool> SatisfyAsync(ExportFormat format, CancellationToken cancellationToken)
    {
        return Task.FromResult(format == ExportFormat.CSV);
    }

    public async Task<Stream> ExportAsync<TModel>(TModel exportModel, ChartData chartData = null, CancellationToken cancellationToken = default)
        where TModel : BaseReport
    {
        return await ExportAsync([exportModel], chartData, cancellationToken);
    }

    public async Task<Stream> ExportAsync<TModel>(IEnumerable<TModel> exportModels, ChartData chartData = null, CancellationToken cancellationToken = default)
        where TModel : BaseReport
    {
        var csvStream = new MemoryStream();
        await using (var writer = new StreamWriter(csvStream, leaveOpen: true))
        {
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.Context.TypeConverterOptionsCache.GetOptions<Guid?>().NullValues.Add("-");
            csv.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add("-");

            csv.Context.RegisterClassMap(ClassMapFactory.GetClassMap<TModel>());
            csv.WriteHeader<TModel>();
            await csv.NextRecordAsync();
            await csv.WriteRecordsAsync(exportModels, cancellationToken);
        }

        csvStream.Seek(0, SeekOrigin.Begin);
        return csvStream;
    }
}
