using System.Globalization;
using CsvHelper.Configuration;
using Exadel.ReportHub.Data.Abstract;
using Exadel.ReportHub.Export.Abstract;

namespace Exadel.ReportHub.Csv.ClassMaps;

public abstract class BaseReportMap<TModel> : ClassMap<TModel>
    where TModel : BaseReport
{
    protected BaseReportMap()
    {
        AutoMap(CultureInfo.InvariantCulture);
        Map(x => x.ReportDate).TypeConverterOption.Format(Constants.Format.Date);
    }
}
