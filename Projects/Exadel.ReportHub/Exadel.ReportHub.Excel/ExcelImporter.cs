using System.Reflection;
using Aspose.Cells;
using Exadel.ReportHub.Excel.Abstract;

namespace Exadel.ReportHub.Excel;

public class ExcelImporter : IExcelImporter
{
    public IList<TDto> Read<TDto>(Stream excelStream)
         where TDto : new()
    {
        using var workbook = new Workbook(excelStream);
        using var worksheet = workbook.Worksheets[0];
        var cells = worksheet.Cells;

        var header = GetHeader(cells);
        var properties = typeof(TDto).GetProperties();

        return ExtractRows<TDto>(cells, header, properties);
    }

    private IDictionary<string, int> GetHeader(Cells cells)
    {
        var header = Enumerable
            .Range(0, cells.MaxDataColumn + 1)
            .Select(column =>
            {
                var header = cells[0, column].StringValue?.Trim();
                return (Header: header, Column: column );
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.Header))
            .DistinctBy(x => x.Header)
            .ToDictionary(x => x.Header, x => x.Column);

        return header;
    }

    private IList<TDto> ExtractRows<TDto>(Cells cells, IDictionary<string, int> headerMap, PropertyInfo[] properties)
        where TDto : new()
    {
        var items = Enumerable
            .Range(1, cells.MaxDataRow)
            .Select(rowIndex =>
            {
                var dto = new TDto();
                var row = cells.Rows[rowIndex];
                PopulateDto(row, dto, headerMap, properties);
                return dto;
            }).ToList();

        return items;
    }

    private void PopulateDto<TDto>(Row row, TDto dto, IDictionary<string, int> headerMap, PropertyInfo[] properties)
    {
        foreach (var property in properties)
        {
            if (!headerMap.TryGetValue(property.Name, out var columnIndex))
            {
                continue;
            }

            var cell = row[columnIndex];
            if (cell.Value == null)
            {
                continue;
            }

            var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            if (targetType == typeof(Guid) || targetType == typeof(Guid?))
            {
                HandleGuidConversion(cell, dto, property, targetType);
            }
            else
            {
                var convertedValue = Convert.ChangeType(cell.Value, targetType);
                property.SetValue(dto, convertedValue);
            }
        }
    }

    private void HandleGuidConversion<TDto>(Cell cell, TDto dto, PropertyInfo property, Type targetType)
    {
        if (cell.Value is string stringValue && Guid.TryParse(stringValue, out var guidValue))
        {
            property.SetValue(dto, guidValue);
        }
        else
        {
            property.SetValue(dto, targetType == typeof(Guid?) ? (Guid?)null : Guid.Empty);
        }
    }
}
