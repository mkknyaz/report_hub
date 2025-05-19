using Exadel.ReportHub.Handlers.Invoice.UpdateOverdueStatus;
using Exadel.ReportHub.RA.Abstract;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Invoice.UpdateOverdueStatus;

[TestFixture]
public class UpdateOverdueInvoicesStatusHandlerTests
{
    private const int ExpectedCount = 5;

    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private Mock<ILogger<UpdateOverdueInvoicesStatusHandler>> _loggerMock;

    private UpdateOverdueInvoicesStatusHandler _handler;

    [SetUp]
    public void Setup()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _loggerMock = new Mock<ILogger<UpdateOverdueInvoicesStatusHandler>>();
        _handler = new UpdateOverdueInvoicesStatusHandler(_invoiceRepositoryMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task UpdateOverdueInvoicesStatus_ValidRequest_ReturnsUnit()
    {
        // Arrange
        _invoiceRepositoryMock
            .Setup(x => x.UpdateOverdueStatusAsync(DateTime.Now.Date, CancellationToken.None))
            .ReturnsAsync(ExpectedCount);

        // Act
        var request = new UpdateOverdueInvoicesStatusRequest();
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(Unit.Value));

        _invoiceRepositoryMock.Verify(
            x => x.UpdateOverdueStatusAsync(DateTime.Now.Date, CancellationToken.None),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Marked {ExpectedCount} invoices as overdue", StringComparison.OrdinalIgnoreCase)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
