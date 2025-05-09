using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Export.Abstract.Models;

namespace Exadel.ReportHub.Csv.ClassMaps;

public class ItemsReportMap : BaseReportMap<ItemsReport>
{
    public ItemsReportMap()
    {
        Map(x => x.AveragePrice).TypeConverterOption.Format(Constants.Format.Decimal);
        Map(x => x.AverageRevenue).TypeConverterOption.Format(Constants.Format.Decimal);
    }
}
