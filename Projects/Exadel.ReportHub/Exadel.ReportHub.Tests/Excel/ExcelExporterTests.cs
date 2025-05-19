using Aspose.Cells;
using AutoFixture;
using Exadel.ReportHub.Data.Abstract;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.Excel;
using Exadel.ReportHub.SDK.Enums;
using Exadel.ReportHub.Tests.Abstracts;

namespace Exadel.ReportHub.Tests.Excel;

[TestFixture]
public class ExcelExporterTests : BaseTestFixture
{
    private ExcelExporter _excelExporter;

    [SetUp]
    public void SetUp()
    {
        _excelExporter = new ExcelExporter();
    }

    [Test]
    public async Task ExportExcel_ValidFormat_ReturnsTrue()
    {
        // Act
        var result = await _excelExporter.SatisfyAsync(ExportFormat.Excel, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExportExcel_InvalidFormat_ReturnsFalse()
    {
        // Act
        var result = await _excelExporter.SatisfyAsync(ExportFormat.CSV, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task ExportExcel_ValidRequest_GeneratesCorrectExcelFile()
    {
        // Arrange
        var report = Fixture.Build<InvoicesReport>()
            .With(x => x.ReportDate, DateTime.UtcNow).Create();
        var reports = new List<InvoicesReport> { report };

        // Act
        var result = await _excelExporter.ExportAsync(reports, null, CancellationToken.None);

        // Assert
        using var workbook = new Workbook(result);
        using var worksheet = workbook.Worksheets[0];

        Assert.That(worksheet.Cells[0, 0].StringValue, Is.EqualTo("ReportDate"));
        Assert.That(worksheet.Cells[0, 1].DateTimeValue.Date, Is.EqualTo(report.ReportDate.Date));

        var properties = typeof(InvoicesReport).GetProperties().Where(p => p.Name != "ReportDate").ToList();
        for (int i = 0; i < properties.Count; i++)
        {
            Assert.That(worksheet.Cells[1, i].StringValue, Is.EqualTo(properties[i].Name));
        }

        for (int i = 0; i < properties.Count; i++)
        {
            var expectedValue = properties[i].GetValue(report);
            var cell = worksheet.Cells[2, i];

            if (expectedValue is decimal or double or float)
            {
                var expectedDouble = Convert.ToDouble(expectedValue);
                Assert.That(cell.DoubleValue, Is.EqualTo(expectedDouble));
            }
            else if (expectedValue is int)
            {
                var expectedInt = Convert.ToInt32(expectedValue);
                Assert.That(cell.IntValue, Is.EqualTo(expectedInt));
            }
            else if (expectedValue is DateTime date)
            {
                Assert.That(cell.DateTimeValue.Date, Is.EqualTo(date.Date));
            }
            else
            {
                var expectedString = expectedValue.ToString();
                Assert.That(cell.StringValue, Is.EqualTo(expectedString));
            }
        }
    }

    [Test]
    public async Task ExportExcel_SingleReportModel_GeneratesCorrectExcelFile()
    {
        // Arrange
        var report = Fixture.Build<InvoicesReport>()
            .With(x => x.ReportDate, DateTime.UtcNow).Create();

        // Act
        var result = await _excelExporter.ExportAsync(report, null, CancellationToken.None);

        // Assert
        using var workbook = new Workbook(result);
        using var worksheet = workbook.Worksheets[0];

        Assert.That(worksheet.Cells[0, 0].StringValue, Is.EqualTo("ReportDate"));
        Assert.That(worksheet.Cells[0, 1].DateTimeValue.Date, Is.EqualTo(report.ReportDate.Date));

        var properties = typeof(InvoicesReport)
            .GetProperties()
            .Where(p => p.Name != nameof(BaseReport.ReportDate))
            .ToList();

        for (int i = 0; i < properties.Count; i++)
        {
            Assert.That(worksheet.Cells[1, i].StringValue, Is.EqualTo(properties[i].Name));
        }

        for (int i = 0; i < properties.Count; i++)
        {
            var expectedValue = properties[i].GetValue(report);
            var cell = worksheet.Cells[2, i];

            if (expectedValue is decimal or double or float)
            {
                var expectedDouble = Convert.ToDouble(expectedValue);
                Assert.That(cell.DoubleValue, Is.EqualTo(expectedDouble));
            }
            else if (expectedValue is int)
            {
                var expectedInt = Convert.ToInt32(expectedValue);
                Assert.That(cell.IntValue, Is.EqualTo(expectedInt));
            }
            else if (expectedValue is DateTime date)
            {
                Assert.That(cell.DateTimeValue.Date, Is.EqualTo(date.Date));
            }
            else
            {
                var expectedString = expectedValue.ToString();
                Assert.That(cell.StringValue, Is.EqualTo(expectedString));
            }
        }
    }
}
