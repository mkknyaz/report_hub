using System.Globalization;
using Aspose.Cells;
using Aspose.Cells.Charts;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Export.Abstract.Models;

namespace Exadel.ReportHub.Excel.Helpers;

public static class ChartPrinter
{
    public static void PrintChart(Worksheet worksheet, ChartData chartData)
    {
        const int chartWidth = 10;
        const int chartHeight = 20;

        var chartIndex = worksheet.Charts.Add(ChartType.Column,
            0, worksheet.Cells.MaxDataColumn + 2,
            chartHeight, worksheet.Cells.MaxDataColumn + 2 + chartWidth);
        var chart = worksheet.Charts[chartIndex];
        chart.Title.Text = chartData.ChartTitle;

        int i = 0;
        foreach (var nameValue in chartData.NameValues)
        {
            string value = $"{{{nameValue.Value.ToString(Constants.Format.Decimal, CultureInfo.InvariantCulture)}}}";
            chart.NSeries.Add(value, true);
            chart.NSeries[i].Name = $"{i + 1}: {nameValue.Key}";
            chart.NSeries[i].DataLabels.ShowValue = true;
            i++;
        }

        chart.NSeries.CategoryData = "ZZ1:ZZ1";
        chart.CategoryAxis.Title.Text = chartData.NamesTitle;
        chart.ValueAxis.Title.Text = chartData.ValuesTitle;
    }
}
