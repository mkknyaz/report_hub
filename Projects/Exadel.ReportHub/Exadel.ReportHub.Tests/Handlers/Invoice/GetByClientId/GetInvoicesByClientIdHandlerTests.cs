using AutoFixture;
using Exadel.ReportHub.Handlers.Invoice.GetByClientId;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Invoice.GetByClientId;

[TestFixture]
public class GetInvoicesByClientIdHandlerTests : BaseTestFixture
{
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private GetInvoicesByClientIdHandler _handler;

    [SetUp]
    public void Setup()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _handler = new GetInvoicesByClientIdHandler(_invoiceRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task GetInvoicesByClientId_ValidRequest_ReturnsInvoiceDtos()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var invoices = Fixture.CreateMany<Data.Models.Invoice>(2).ToList();
        var invoiceDtos = Mapper.Map<List<InvoiceDTO>>(invoices);

        _invoiceRepositoryMock
            .Setup(r => r.GetByClientIdAsync(clientId, CancellationToken.None))
            .ReturnsAsync(invoices);

        // Act
        var result = await _handler.Handle(new GetInvoicesByClientIdRequest(clientId), CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value[0].Id, Is.EqualTo(invoiceDtos[0].Id));
        Assert.That(result.Value[0].ClientId, Is.EqualTo(invoiceDtos[0].ClientId));
        Assert.That(result.Value[0].CustomerId, Is.EqualTo(invoiceDtos[0].CustomerId));
        Assert.That(result.Value[0].InvoiceNumber, Is.EqualTo(invoiceDtos[0].InvoiceNumber));
        Assert.That(result.Value[0].IssueDate, Is.EqualTo(invoiceDtos[0].IssueDate));
        Assert.That(result.Value[0].DueDate, Is.EqualTo(invoiceDtos[0].DueDate));
        Assert.That(result.Value[0].ClientBankAccountNumber, Is.EqualTo(invoiceDtos[0].ClientBankAccountNumber));
        Assert.That(result.Value[0].ClientCurrencyId, Is.EqualTo(invoiceDtos[0].ClientCurrencyId));
        Assert.That(result.Value[0].ClientCurrencyCode, Is.EqualTo(invoiceDtos[0].ClientCurrencyCode));
        Assert.That(result.Value[0].ClientCurrencyAmount, Is.EqualTo(invoiceDtos[0].ClientCurrencyAmount));
        Assert.That(result.Value[0].CustomerCurrencyId, Is.EqualTo(invoiceDtos[0].CustomerCurrencyId));
        Assert.That(result.Value[0].CustomerCurrencyCode, Is.EqualTo(invoiceDtos[0].CustomerCurrencyCode));
        Assert.That(result.Value[0].CustomerCurrencyAmount, Is.EqualTo(invoiceDtos[0].CustomerCurrencyAmount));
        Assert.That(result.Value[0].PaymentStatus, Is.EqualTo(invoiceDtos[0].PaymentStatus));
        Assert.That(result.Value[0].ItemIds, Is.EquivalentTo(invoiceDtos[0].ItemIds));

        Assert.That(result.Value[1].Id, Is.EqualTo(invoiceDtos[1].Id));
        Assert.That(result.Value[1].ClientId, Is.EqualTo(invoiceDtos[1].ClientId));
        Assert.That(result.Value[1].CustomerId, Is.EqualTo(invoiceDtos[1].CustomerId));
        Assert.That(result.Value[1].InvoiceNumber, Is.EqualTo(invoiceDtos[1].InvoiceNumber));
        Assert.That(result.Value[1].IssueDate, Is.EqualTo(invoiceDtos[1].IssueDate));
        Assert.That(result.Value[1].DueDate, Is.EqualTo(invoiceDtos[1].DueDate));
        Assert.That(result.Value[1].ClientBankAccountNumber, Is.EqualTo(invoiceDtos[1].ClientBankAccountNumber));
        Assert.That(result.Value[1].ClientCurrencyId, Is.EqualTo(invoiceDtos[1].ClientCurrencyId));
        Assert.That(result.Value[1].ClientCurrencyCode, Is.EqualTo(invoiceDtos[1].ClientCurrencyCode));
        Assert.That(result.Value[1].ClientCurrencyAmount, Is.EqualTo(invoiceDtos[1].ClientCurrencyAmount));
        Assert.That(result.Value[1].CustomerCurrencyId, Is.EqualTo(invoiceDtos[1].CustomerCurrencyId));
        Assert.That(result.Value[1].CustomerCurrencyCode, Is.EqualTo(invoiceDtos[1].CustomerCurrencyCode));
        Assert.That(result.Value[1].CustomerCurrencyAmount, Is.EqualTo(invoiceDtos[1].CustomerCurrencyAmount));
        Assert.That(result.Value[1].PaymentStatus, Is.EqualTo(invoiceDtos[1].PaymentStatus));
        Assert.That(result.Value[1].ItemIds, Is.EquivalentTo(invoiceDtos[1].ItemIds));

        _invoiceRepositoryMock.Verify(r => r.GetByClientIdAsync(clientId, CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task GetInvoicesByClientId_WhenNoInvoicesExist_ReturnsEmpty()
    {
        // Arrange
        var clientId = Guid.NewGuid();

        _invoiceRepositoryMock
            .Setup(r => r.GetByClientIdAsync(clientId, CancellationToken.None))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(new GetInvoicesByClientIdRequest(clientId), CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Empty);

        _invoiceRepositoryMock.Verify(r => r.GetByClientIdAsync(clientId, CancellationToken.None), Times.Once);
    }
}
