using ErrorOr;
using Exadel.ReportHub.Handlers.Invoice.UpdatePaidStatus;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Invoice.UpdatePaidStatus;

[TestFixture]
public class UpdateInvoicePaidStatusHandlerTests : BaseTestFixture
{
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;

    private UpdateInvoicePaidStatusHandler _handler;

    [SetUp]
    public void Setup()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _handler = new UpdateInvoicePaidStatusHandler(_invoiceRepositoryMock.Object);
    }

    [Test]
    public async Task UpdateInvoice_ValidRequest_ReturnsUpdated()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        _invoiceRepositoryMock
            .Setup(x => x.ExistsAsync(invoiceId, clientId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var request = new UpdateInvoicePaidStatusRequest(invoiceId, clientId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.EqualTo(Result.Updated));

        _invoiceRepositoryMock.Verify(
            x => x.UpdatePaidStatusAsync(invoiceId, clientId, CancellationToken.None),
            Times.Once);

        _invoiceRepositoryMock.Verify(
            x => x.ExistsAsync(invoiceId, clientId, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task UpdateInvoice_CustomerNotFound_ReturnsNotFound()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        _invoiceRepositoryMock
            .Setup(x => x.ExistsAsync(invoiceId, clientId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var request = new UpdateInvoicePaidStatusRequest(invoiceId, clientId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _invoiceRepositoryMock.Verify(
            x => x.ExistsAsync(invoiceId, clientId, CancellationToken.None),
            Times.Once);

        _invoiceRepositoryMock.Verify(
            x => x.UpdatePaidStatusAsync(invoiceId, clientId, CancellationToken.None),
            Times.Never);
    }
}
