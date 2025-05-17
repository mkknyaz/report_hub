using AutoFixture;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.Handlers.Invoice.GetRevenue;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Invoice.GetRevenue;

[TestFixture]
public class GetInvoicesRevenueHandlerTests : BaseTestFixture
{
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private Mock<IClientRepository> _clientRepositoryMock;

    private GetInvoicesRevenueHandler _handler;

    [SetUp]
    public void Setup()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();

        _handler = new GetInvoicesRevenueHandler(
            _invoiceRepositoryMock.Object,
            _clientRepositoryMock.Object,
            Mapper);
    }

    [Test]
    public async Task GetInvoicesRevenue_WhenClientHasInvoices_ReturnsTotalInvoicesRevenueDto()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var invoiceRevenueFilterDto = Fixture.Build<InvoiceRevenueFilterDTO>().With(x => x.ClientId, clientId).Create();
        var totalRevenue = Fixture.Create<TotalRevenue>();

        _invoiceRepositoryMock
            .Setup(r => r.GetTotalAmountByDateRangeAsync(
                clientId,
                invoiceRevenueFilterDto.StartDate,
                invoiceRevenueFilterDto.EndDate,
                CancellationToken.None))
            .ReturnsAsync(totalRevenue);

        // Act
        var request = new GetInvoicesRevenueRequest(invoiceRevenueFilterDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.TotalAmount, Is.EqualTo(totalRevenue.TotalAmount));
        Assert.That(result.Value.CurrencyCode, Is.EqualTo(totalRevenue.CurrencyCode));

        _invoiceRepositoryMock
            .Verify(r => r.GetTotalAmountByDateRangeAsync(
                clientId,
                invoiceRevenueFilterDto.StartDate,
                invoiceRevenueFilterDto.EndDate,
                CancellationToken.None),
                Times.Once);
    }

    [Test]
    public async Task GetInvoicesRevenue_WhenClientHasNoInvoices_ReturnsEmptyDto()
    {
        // Arrange
        var currency = "EUR";
        var clientId = Guid.NewGuid();

        var invoiceRevenueFilterDto = Fixture.Build<InvoiceRevenueFilterDTO>().With(x => x.ClientId, clientId).Create();

        _invoiceRepositoryMock
            .Setup(r => r.GetTotalAmountByDateRangeAsync(
                clientId,
                invoiceRevenueFilterDto.StartDate,
                invoiceRevenueFilterDto.EndDate,
                CancellationToken.None))
            .ReturnsAsync((TotalRevenue)null);

        _clientRepositoryMock
            .Setup(r => r.GetCurrencyAsync(clientId, CancellationToken.None))
            .ReturnsAsync(currency);

        // Act
        var request = new GetInvoicesRevenueRequest(invoiceRevenueFilterDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value.TotalAmount, Is.EqualTo(0m));
        Assert.That(result.Value.CurrencyCode, Is.EqualTo(currency));

        _invoiceRepositoryMock.Verify(
            r => r.GetTotalAmountByDateRangeAsync(
                clientId,
                invoiceRevenueFilterDto.StartDate,
                invoiceRevenueFilterDto.EndDate,
                CancellationToken.None),
            Times.Once);

        _clientRepositoryMock.Verify(r => r.GetCurrencyAsync(clientId, CancellationToken.None), Times.Once);
    }
}
