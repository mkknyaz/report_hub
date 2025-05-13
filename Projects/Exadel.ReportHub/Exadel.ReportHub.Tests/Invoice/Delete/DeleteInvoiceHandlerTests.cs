using ErrorOr;
using Exadel.ReportHub.Handlers.Invoice.Delete;
using Exadel.ReportHub.RA.Abstract;
using Moq;

namespace Exadel.ReportHub.Tests.Invoice.Delete;

[TestFixture]
public class DeleteInvoiceHandlerTests
{
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private DeleteInvoiceHandler _handler;

    [SetUp]
    public void Setup()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _handler = new DeleteInvoiceHandler(_invoiceRepositoryMock.Object);
    }

    [Test]
    public async Task DeleteInvoice_WhenExists_ReturnsDeleted()
    {
        // Arrange
        var id = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        _invoiceRepositoryMock
            .Setup(r => r.ExistsAsync(id, clientId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(new DeleteInvoiceRequest(id, clientId), CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.EqualTo(Result.Deleted));

        _invoiceRepositoryMock.Verify(r => r.ExistsAsync(id, clientId, CancellationToken.None), Times.Once);
        _invoiceRepositoryMock.Verify(r => r.SoftDeleteAsync(id, CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task DeleteInvoice_WhenNotExist_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        _invoiceRepositoryMock
            .Setup(r => r.ExistsAsync(id, clientId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(new DeleteInvoiceRequest(id, clientId), CancellationToken.None);

        // Assert
        Assert.That(result.Errors, Has.Count.EqualTo(1), "Should contains the only error");
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _invoiceRepositoryMock.Verify(r => r.ExistsAsync(id, clientId, CancellationToken.None), Times.Once);
        _invoiceRepositoryMock.Verify(r => r.SoftDeleteAsync(id, It.IsAny<CancellationToken>()), Times.Never);
    }
}
