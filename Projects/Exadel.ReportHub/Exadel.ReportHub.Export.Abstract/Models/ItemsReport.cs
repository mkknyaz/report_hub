using Exadel.ReportHub.Data.Abstract;

namespace Exadel.ReportHub.Export.Abstract.Models;

public class ItemsReport : BaseReport
{
    public string MostSoldItemName { get; set; }

    public decimal AveragePrice { get; set; }

    public decimal AverageRevenue { get; set; }

    public string ClientCurrency { get; set; }
}
