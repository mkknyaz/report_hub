namespace Exadel.ReportHub.Export.Abstract.Models;

public class ChartData
{
    public string ChartTitle { get; set; }

    public string NamesTitle { get; set; }

    public string ValuesTitle { get; set; }

    public Dictionary<string, decimal> NameValues { get; set; }
}
