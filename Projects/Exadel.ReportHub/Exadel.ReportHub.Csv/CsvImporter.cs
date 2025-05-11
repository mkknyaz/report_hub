using System.Globalization;
using CsvHelper;
using Exadel.ReportHub.Csv.Abstract;
using Exadel.ReportHub.Csv.ClassMaps;

namespace Exadel.ReportHub.Csv;

public class CsvImporter : ICsvImporter
{
    public IList<TResult> Read<TResult>(Stream csvStream)
    {
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap(ClassMapFactory.GetClassMap<TResult>());

        return csv.GetRecords<TResult>().ToList();
    }
}
