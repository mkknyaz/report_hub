using System.Net.Mime;
using AutoFixture;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.Ecb.Abstract;
using Exadel.ReportHub.Export.Abstract;
using Exadel.ReportHub.Export.Abstract.Models;
using Exadel.ReportHub.Handlers.Managers.Report;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Report;
using Exadel.ReportHub.SDK.Enums;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Managers.Report;

[TestFixture]
public class ReportManagerTests : BaseTestFixture
{
    private const string Currency = "EUR";

    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private Mock<IItemRepository> _itemRepositoryMock;
    private Mock<IPlanRepository> _planRepositoryMock;
    private Mock<IClientRepository> _clientRepositoryMock;
    private Mock<ICurrencyConverter> _currencyConverterMock;
    private Mock<IExportStrategyFactory> _exportStrategyFactoryMock;
    private Mock<IExportStrategy> _exportStrategyMock;

    private ReportManager _reportManager;

    [SetUp]
    public void Setup()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _itemRepositoryMock = new Mock<IItemRepository>();
        _planRepositoryMock = new Mock<IPlanRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _currencyConverterMock = new Mock<ICurrencyConverter>();
        _exportStrategyFactoryMock = new Mock<IExportStrategyFactory>();
        _exportStrategyMock = new Mock<IExportStrategy>();

        _reportManager = new ReportManager(
            _invoiceRepositoryMock.Object,
            _itemRepositoryMock.Object,
            _planRepositoryMock.Object,
            _clientRepositoryMock.Object,
            _currencyConverterMock.Object,
            _exportStrategyFactoryMock.Object);
    }

    [Test]
    public async Task GenerateInvoicesReportAsync_ValidRequest_ReturnsExportResult()
    {
        // Arrange
        var exportReportDto = Fixture.Build<ExportReportDTO>()
            .With(x => x.Format, ExportFormat.CSV)
            .Create();

        var report = Fixture.Create<InvoicesReport>();
        var stream = new MemoryStream();

        _exportStrategyFactoryMock.Setup(x => x.GetStrategyAsync(exportReportDto.Format, CancellationToken.None))
            .ReturnsAsync(_exportStrategyMock.Object);

        _invoiceRepositoryMock.Setup(x => x.GetReportAsync(
                exportReportDto.ClientId,
                exportReportDto.StartDate,
                exportReportDto.EndDate,
                CancellationToken.None))
            .ReturnsAsync(report);

        _clientRepositoryMock.Setup(x => x.GetCurrencyAsync(exportReportDto.ClientId, CancellationToken.None))
            .ReturnsAsync(Currency);

        _exportStrategyMock.Setup(x => x.ExportAsync(report, It.IsAny<ChartData>(), CancellationToken.None))
            .ReturnsAsync(stream);

        // Act
        var result = await _reportManager.GenerateInvoicesReportAsync(exportReportDto, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Stream, Is.EqualTo(stream));
        Assert.That(result.FileName, Does.StartWith("InvoicesReport_"));
        Assert.That(result.FileName, Does.EndWith(Constants.File.Extension.Csv));
        Assert.That(result.ContentType, Is.EqualTo(MediaTypeNames.Text.Csv));
        Assert.That(result.FileName, Is.EqualTo($"InvoicesReport_{DateTime.Now:yyyy-MM-dd}.csv"));

        _exportStrategyFactoryMock.Verify(
            x => x.GetStrategyAsync(exportReportDto.Format, CancellationToken.None),
            Times.Once);
        _invoiceRepositoryMock.Verify(
            x => x.GetReportAsync(
                exportReportDto.ClientId,
                exportReportDto.StartDate,
                exportReportDto.EndDate,
                CancellationToken.None),
            Times.Once);
        _clientRepositoryMock.Verify(
            x => x.GetCurrencyAsync(exportReportDto.ClientId, CancellationToken.None),
            Times.Once);
        _exportStrategyMock.Verify(
            x => x.ExportAsync(report, It.IsAny<ChartData>(), CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task GenerateItemsReportAsync_ValidRequest_ReturnsExportResult()
    {
        // Arrange
        var exportReportDto = Fixture.Build<ExportReportDTO>()
            .With(x => x.Format, ExportFormat.Excel)
            .Create();
        var itemNamePrice = Fixture.Build<ItemNamePrice>()
            .With(x => x.Currency, Currency)
            .With(x => x.Price, 10.00m)
            .CreateMany(1).ToList();

        itemNamePrice.Add(
            Fixture.Build<ItemNamePrice>()
                .With(x => x.Currency, Currency)
                .With(x => x.Price, 20.00m)
                .Create());

        var clientItemNamesPrices = new Dictionary<Guid, ItemNamePrice>();

        for (int i = 0; i < 2; i++)
        {
            clientItemNamesPrices.Add(Guid.NewGuid(), itemNamePrice[i]);
        }

        var itemids = clientItemNamesPrices.Keys.ToList();
        var counts = new Dictionary<Guid, int>
        {
            [itemids[0]] = 5,
            [itemids[1]] = 10
        };

        var stream = new MemoryStream();

        _exportStrategyFactoryMock.Setup(x => x.GetStrategyAsync(exportReportDto.Format, CancellationToken.None))
            .ReturnsAsync(_exportStrategyMock.Object);

        _itemRepositoryMock.Setup(x => x.GetClientItemNamesPricesAsync(exportReportDto.ClientId, CancellationToken.None))
            .ReturnsAsync(clientItemNamesPrices);

        _invoiceRepositoryMock.Setup(x => x.GetClientItemsCountAsync(
                exportReportDto.ClientId,
                exportReportDto.StartDate,
                exportReportDto.EndDate,
                CancellationToken.None))
            .ReturnsAsync(counts);

        _clientRepositoryMock.Setup(x => x.GetCurrencyAsync(exportReportDto.ClientId, CancellationToken.None))
            .ReturnsAsync(Currency);

        _exportStrategyMock.Setup(x => x.ExportAsync(It.IsAny<ItemsReport>(), It.IsAny<ChartData>(), CancellationToken.None))
            .ReturnsAsync(stream);

        // Act
        var result = await _reportManager.GenerateItemsReportAsync(exportReportDto, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Stream, Is.EqualTo(stream));
        Assert.That(result.FileName, Does.StartWith("ItemsReport_"));
        Assert.That(result.FileName, Does.EndWith(Constants.File.Extension.Excel));
        Assert.That(result.ContentType, Is.EqualTo(Constants.File.ContentType.Excel));
        Assert.That(result.FileName, Is.EqualTo($"ItemsReport_{DateTime.Now:yyyy-MM-dd}.xlsx"));

        _exportStrategyMock.Verify(
            x => x.ExportAsync(
                It.Is<ItemsReport>(r =>
                    r.MostSoldItemName == clientItemNamesPrices[counts.MaxBy(x => x.Value).Key].Name &&
                    r.ClientCurrency == Currency),
                It.IsAny<ChartData>(),
                CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task GeneratePlansReportAsync_ValidRequest_ReturnsExportResult()
    {
        // Arrange
        var exportReportDto = Fixture.Build<ExportReportDTO>()
            .With(x => x.Format, ExportFormat.CSV)
            .Create();

        var plans = Fixture.CreateMany<Data.Models.Plan>(3).ToList();

        var counts = plans.ToDictionary(x => x.Id, x => x.Count);
        var prices = plans.ToDictionary(
            x => x.ItemId,
            x => Fixture.Build<ItemNamePrice>()
                .With(p => p.Currency, Currency)
                .Create());

        var expectedRevenues = plans.Select(plan =>
        {
            var price = prices[plan.ItemId].Price;
            return new
            {
                PlanId = plan.Id,
                ExpectedRevenue = price * plan.Count,
                prices[plan.ItemId].Currency
            };
        }).ToList();

        var stream = new MemoryStream();

        _exportStrategyFactoryMock.Setup(x => x.GetStrategyAsync(exportReportDto.Format, CancellationToken.None))
            .ReturnsAsync(_exportStrategyMock.Object);

        _planRepositoryMock.Setup(x => x.GetByClientIdAsync(
                exportReportDto.ClientId,
                exportReportDto.StartDate,
                exportReportDto.EndDate,
                CancellationToken.None))
            .ReturnsAsync(plans);

        _invoiceRepositoryMock.Setup(x => x.GetPlansActualCountAsync(plans, CancellationToken.None))
            .ReturnsAsync(counts);

        _itemRepositoryMock.Setup(x => x.GetClientItemNamesPricesAsync(exportReportDto.ClientId, CancellationToken.None))
            .ReturnsAsync(prices);

        _clientRepositoryMock.Setup(x => x.GetCurrencyAsync(exportReportDto.ClientId, CancellationToken.None))
            .ReturnsAsync(Currency);

        _exportStrategyMock.Setup(x => x.ExportAsync(It.IsAny<List<PlanReport>>(), It.IsAny<ChartData>(), CancellationToken.None))
            .ReturnsAsync(stream);

        // Act
        var result = await _reportManager.GeneratePlansReportAsync(exportReportDto, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Stream, Is.EqualTo(stream));
        Assert.That(result.FileName, Does.StartWith("PlansReport_"));
        Assert.That(result.FileName, Does.EndWith(Constants.File.Extension.Csv));
        Assert.That(result.ContentType, Is.EqualTo(MediaTypeNames.Text.Csv));

        _exportStrategyMock.Verify(
            x => x.ExportAsync(
                It.Is<List<PlanReport>>(reports =>
                    reports.Count == 3 &&
                    reports.All(r =>
                        expectedRevenues.Any(e =>
                            e.ExpectedRevenue == r.Revenue &&
                            e.Currency == r.ItemCurrency))),
                It.IsAny<ChartData>(),
                CancellationToken.None),
            Times.Once);
    }
}
