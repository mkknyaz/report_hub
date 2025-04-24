using System.Globalization;
using CsvHelper;
using Exadel.ReportHub.Csv.Abstract;
using Exadel.ReportHub.Csv.ClassMaps;
using Exadel.ReportHub.SDK.DTOs.Invoice;

namespace Exadel.ReportHub.Csv;

public class CsvProcessor : ICsvProcessor
{
    public IList<CreateInvoiceDTO> ReadInvoices(Stream csvStream)
    {
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<CreateInvoiceMap>();

        return csv.GetRecords<CreateInvoiceDTO>().ToList();
    }
}
