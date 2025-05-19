using AutoFixture;
using ErrorOr;
using Exadel.ReportHub.Handlers.Invoice.Update;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Invoice.Update;

[TestFixture]
public class UpdateInvoiceHandlerTests : BaseTestFixture
{
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;

    private UpdateInvoiceHandler _handler;

    [SetUp]
    public void Setup()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();

        _handler = new UpdateInvoiceHandler(_invoiceRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task UpdateInvoice_ValidRequest_ReturnsUpdated()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var updateDto = Fixture.Create<UpdateInvoiceDTO>();

        _invoiceRepositoryMock
            .Setup(x => x.ExistsAsync(invoiceId, clientId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var request = new UpdateInvoiceRequest(invoiceId, clientId, updateDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.EqualTo(Result.Updated));

        _invoiceRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.Is<Data.Models.Invoice>(i =>
                    i.IssueDate == updateDto.IssueDate &&
                    i.DueDate == updateDto.DueDate),
                CancellationToken.None),
            Times.Once);

        _invoiceRepositoryMock.Verify(
            x => x.ExistsAsync(invoiceId, clientId, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task UpdateInvoice_CustomerNotFound_ReturnsNotFound()
    {
        // Arrange
        var invoice = Fixture.Create<Data.Models.Invoice>();
        var updateDto = Fixture.Create<UpdateInvoiceDTO>();

        _invoiceRepositoryMock
            .Setup(x => x.ExistsAsync(invoice.Id, invoice.ClientId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var request = new UpdateInvoiceRequest(invoice.Id, invoice.ClientId, updateDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _invoiceRepositoryMock.Verify(
            x => x.ExistsAsync(invoice.Id, invoice.ClientId, CancellationToken.None),
            Times.Once);

        _invoiceRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Data.Models.Invoice>(), CancellationToken.None),
            Times.Never);
    }
}
