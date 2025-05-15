using System.Reflection;
using Aspose.Cells;
using Exadel.ReportHub.Data.Abstract;
using Exadel.ReportHub.Excel.Helpers;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Export.Abstract.Models;
using Exadel.ReportHub.SDK.Enums;

namespace Exadel.ReportHub.Excel;

public class ExcelExporter : IExportStrategy
{
    public Task<bool> SatisfyAsync(ExportFormat format, CancellationToken cancellationToken)
    {
        return Task.FromResult(format == ExportFormat.Excel);
    }

    public async Task<Stream> ExportAsync<TModel>(TModel exportModel, ChartData chartData = null, CancellationToken cancellationToken = default)
        where TModel : BaseReport
    {
        return await ExportAsync([exportModel], chartData, cancellationToken);
    }

    public async Task<Stream> ExportAsync<TModel>(IEnumerable<TModel> exportModels, ChartData chartData = null, CancellationToken cancellationToken = default)
        where TModel : BaseReport
    {
        var stream = new MemoryStream();
        const string worksheetName = "Report";

        using var workbook = new Workbook();
        using var worksheet = workbook.Worksheets[0];
        worksheet.Name = worksheetName;

        var dateStyle = workbook.CreateStyle();
        dateStyle.Custom = Constants.Format.Date;
        var decimalStyle = workbook.CreateStyle();
        decimalStyle.Custom = Constants.Format.Decimal;

        var cells = worksheet.Cells;
        var properties = typeof(TModel).GetProperties();
        var modelList = exportModels.ToList();

        cells[0, 0].PutValue(nameof(BaseReport.ReportDate));
        cells[0, 1].PutValue(modelList.FirstOrDefault()?.ReportDate ?? DateTime.UtcNow);
        cells[0, 1].SetStyle(dateStyle);

        PutHeaders(cells, properties);
        PutData(cells, properties, modelList, dateStyle, decimalStyle);

        if (modelList.Count > 0 && chartData != null)
        {
            ChartPrinter.PrintChart(worksheet, chartData);
        }

        worksheet.AutoFitColumns();
        worksheet.AutoFitRows();

        await workbook.SaveAsync(stream, SaveFormat.Xlsx);

        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    private void PutHeaders(Cells cells, IList<PropertyInfo> properties)
    {
        var column = 0;
        foreach (var propertyName in properties.Select(x => x.Name))
        {
            if (propertyName.Equals(nameof(BaseReport.ReportDate), StringComparison.Ordinal))
            {
                continue;
            }

            cells[1, column].PutValue(propertyName);
            column++;
        }
    }

    private void PutData<TModel>(Cells cells, IList<PropertyInfo> properties, IList<TModel> modelList, Style dateStyle, Style decimalStyle)
    {
        var row = 2;
        foreach (var model in modelList)
        {
            var column = 0;
            foreach (var property in properties)
            {
                if (property.Name.Equals(nameof(BaseReport.ReportDate), StringComparison.Ordinal))
                {
                    continue;
                }

                var value = property.GetValue(model);
                cells[row, column].PutValue(value);

                if (value is null)
                {
                    cells[row, column].PutValue("-");
                }

                if (value is DateTime)
                {
                    cells[row, column].SetStyle(dateStyle);
                }

                if (value is decimal)
                {
                    cells[row, column].SetStyle(decimalStyle);
                }

                column++;
            }

            row++;
        }
    }
}
