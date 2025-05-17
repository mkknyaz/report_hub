using System.Globalization;
using System.Text;
using AutoFixture;
using Exadel.ReportHub.Csv;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.SDK.Enums;
using Exadel.ReportHub.Tests.Abstracts;

namespace Exadel.ReportHub.Tests.Csv;

[TestFixture]
public class CsvExporterTests : BaseTestFixture
{
    private CsvExporter _csvExporter;

    [SetUp]
    public void SetUp()
    {
        _csvExporter = new CsvExporter();
    }

    [Test]
    public async Task ExportCsv_ValidFormat_ReturnsTrue()
    {
        // Act
        var result = await _csvExporter.SatisfyAsync(ExportFormat.CSV, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExportCsv_InvalidFormat_ReturnsTrue()
    {
        // Act
        var result = await _csvExporter.SatisfyAsync(ExportFormat.Excel, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task ExportCsv_ValidRequest_ReturnsStreamContainingExpectedHeadersAndValues()
    {
        // Arrange
        var report = Fixture.Create<InvoicesReport>();

        // Act
        var stream = await _csvExporter.ExportAsync(report, null, CancellationToken.None);

        // Assert
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var csv = await reader.ReadToEndAsync();

        Assert.That(csv, Does.Contain(nameof(report.TotalCount)));
        Assert.That(csv, Does.Contain(nameof(report.AverageMonthCount)));
        Assert.That(csv, Does.Contain(nameof(report.TotalAmount)));
        Assert.That(csv, Does.Contain(nameof(report.ClientCurrency)));
        Assert.That(csv, Does.Contain(nameof(report.UnpaidCount)));
        Assert.That(csv, Does.Contain(nameof(report.OverdueCount)));
        Assert.That(csv, Does.Contain(nameof(report.PaidOnTimeCount)));
        Assert.That(csv, Does.Contain(nameof(report.PaidOnTimeCount)));

        Assert.That(csv, Does.Contain(report.TotalCount.ToString()));
        Assert.That(csv, Does.Contain(report.AverageMonthCount.ToString()));
        Assert.That(csv, Does.Contain(report.TotalAmount.ToString("G", CultureInfo.InvariantCulture)));
        Assert.That(csv, Does.Contain(report.AverageAmount.ToString("G", CultureInfo.InvariantCulture)));
        Assert.That(csv, Does.Contain(report.ClientCurrency));
        Assert.That(csv, Does.Contain(report.UnpaidCount.ToString()));
        Assert.That(csv, Does.Contain(report.OverdueCount.ToString()));
        Assert.That(csv, Does.Contain(report.PaidOnTimeCount.ToString()));
        Assert.That(csv, Does.Contain(report.PaidLateCount.ToString()));
    }

    [Test]
    public async Task ExportCsv_ValidRequest_ReturnsCorrectNumberOfLinesOfMultipleReports()
    {
        // Arrange
        var reports = Fixture.CreateMany<InvoicesReport>(3).ToList();

        // Act
        var stream = await _csvExporter.ExportAsync(reports, null, CancellationToken.None);

        // Assert
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var csv = await reader.ReadToEndAsync();
        var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.That(lines.Length, Is.EqualTo(4));
    }
}
