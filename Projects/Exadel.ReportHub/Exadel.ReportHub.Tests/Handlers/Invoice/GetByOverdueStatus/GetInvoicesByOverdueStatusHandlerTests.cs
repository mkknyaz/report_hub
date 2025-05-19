using AutoFixture;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.Handlers.Invoice.GetByOverdueStatus;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Invoice.GetByOverdueStatus;

[TestFixture]
public class GetInvoicesByOverdueStatusHandlerTests : BaseTestFixture
{
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private Mock<IClientRepository> _clientRepositoryMock;
    private GetInvoicesByOverdueStatusHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _handler = new GetInvoicesByOverdueStatusHandler(
            _invoiceRepositoryMock.Object,
            _clientRepositoryMock.Object,
            Mapper);
    }

    [Test]
    public async Task GetInvoicesByOverdueStatus_WhenNoOverdue_ReturnsEmptyDto()
    {
        // Arrange
        var currency = "EUR";
        var clientId = Guid.NewGuid();

        _invoiceRepositoryMock
            .Setup(r => r.GetOverdueAsync(clientId, CancellationToken.None))
            .ReturnsAsync((OverdueCount)null);

        _clientRepositoryMock
            .Setup(r => r.GetCurrencyAsync(clientId, CancellationToken.None))
            .ReturnsAsync(currency);

        // Act
        var result = await _handler.Handle(new GetInvoicesByOverdueStatusRequest(clientId), CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value.Count, Is.EqualTo(0));
        Assert.That(result.Value.TotalAmount, Is.EqualTo(0m));
        Assert.That(result.Value.ClientCurrencyCode, Is.EqualTo(currency));

        _invoiceRepositoryMock.Verify(r => r.GetOverdueAsync(clientId, CancellationToken.None), Times.Once);
        _clientRepositoryMock.Verify(r => r.GetCurrencyAsync(clientId, CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task GetInvoicesByOverdueStatus_WhenHasOverdue_ReturnsDto()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var overdueCount = Fixture.Create<OverdueCount>();
        var overdueCountDto = Mapper.Map<OverdueInvoicesResultDTO>(overdueCount);

        _invoiceRepositoryMock
            .Setup(r => r.GetOverdueAsync(clientId, CancellationToken.None))
            .ReturnsAsync(overdueCount);

        // Act
        var result = await _handler.Handle(new GetInvoicesByOverdueStatusRequest(clientId), CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value.Count, Is.EqualTo(overdueCountDto.Count));
        Assert.That(result.Value.TotalAmount, Is.EqualTo(overdueCountDto.TotalAmount));
        Assert.That(result.Value.ClientCurrencyCode, Is.EqualTo(overdueCountDto.ClientCurrencyCode));

        _invoiceRepositoryMock.Verify(r => r.GetOverdueAsync(clientId, CancellationToken.None), Times.Once);
    }
}
