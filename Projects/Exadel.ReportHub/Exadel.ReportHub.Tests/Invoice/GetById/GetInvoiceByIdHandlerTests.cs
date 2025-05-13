using AutoFixture;
using ErrorOr;
using Exadel.ReportHub.Handlers.Invoice.GetById;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Invoice.GetById;

[TestFixture]
public class GetInvoiceByIdHandlerTests : BaseTestFixture
{
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private GetInvoiceByIdHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _handler = new GetInvoiceByIdHandler(_invoiceRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task GetInvoiceById_WhenExists_ReturnsDto()
    {
        // Arrange
        var invoice = Fixture.Create<Data.Models.Invoice>();
        var expectedDto = Mapper.Map<InvoiceDTO>(invoice);

        _invoiceRepositoryMock
            .Setup(r => r.GetByIdAsync(invoice.Id, invoice.ClientId, CancellationToken.None))
            .ReturnsAsync(invoice);

        // Act
        var result = await _handler.Handle(new GetInvoiceByIdRequest(invoice.Id, invoice.ClientId), CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Id, Is.EqualTo(expectedDto.Id));
        Assert.That(result.Value.ClientId, Is.EqualTo(expectedDto.ClientId));
        Assert.That(result.Value.CustomerId, Is.EqualTo(expectedDto.CustomerId));
        Assert.That(result.Value.InvoiceNumber, Is.EqualTo(expectedDto.InvoiceNumber));
        Assert.That(result.Value.IssueDate, Is.EqualTo(expectedDto.IssueDate));
        Assert.That(result.Value.DueDate, Is.EqualTo(expectedDto.DueDate));
        Assert.That(result.Value.ItemIds, Is.EquivalentTo(expectedDto.ItemIds));
        Assert.That(result.Value.PaymentStatus, Is.EqualTo(expectedDto.PaymentStatus));

        _invoiceRepositoryMock.Verify(r => r.GetByIdAsync(invoice.Id, invoice.ClientId, CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task GetInvoiceById_WhenNotFound_ReturnsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        _invoiceRepositoryMock
            .Setup(r => r.GetByIdAsync(id, clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Data.Models.Invoice)null);

        // Act
        var result = await _handler.Handle(new GetInvoiceByIdRequest(id, clientId), CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _invoiceRepositoryMock.Verify(r => r.GetByIdAsync(id, clientId, CancellationToken.None), Times.Once);
    }
}
