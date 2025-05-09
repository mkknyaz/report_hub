using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.Export.Abstract;

namespace Exadel.ReportHub.Csv.ClassMaps;

public class InvoicesReportMap : BaseReportMap<InvoicesReport>
{
    public InvoicesReportMap()
    {
        Map(x => x.TotalAmount).TypeConverterOption.Format(Constants.Format.Decimal);
        Map(x => x.AverageAmount).TypeConverterOption.Format(Constants.Format.Decimal);
    }
}
