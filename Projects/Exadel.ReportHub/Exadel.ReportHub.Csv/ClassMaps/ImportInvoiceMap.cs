using System.Globalization;
using CsvHelper.Configuration;
using Exadel.ReportHub.SDK.DTOs.Invoice;

namespace Exadel.ReportHub.Csv.ClassMaps;

public class ImportInvoiceMap : ClassMap<ImportInvoiceDTO>
{
    public ImportInvoiceMap()
    {
        AutoMap(CultureInfo.InvariantCulture);

        Map(x => x.ItemIds)
            .Convert(args => args.Row.GetField(nameof(ImportInvoiceDTO.ItemIds)).Split(";").Select(Guid.Parse).ToList());
    }
}
