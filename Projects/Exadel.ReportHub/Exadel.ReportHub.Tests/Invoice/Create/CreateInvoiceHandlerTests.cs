using AutoFixture;
using Exadel.ReportHub.Handlers.Invoice.Create;
using Exadel.ReportHub.Handlers.Managers.Invoice;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using Exadel.ReportHub.SDK.Enums;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Invoice.Create;

[TestFixture]
public class CreateInvoiceHandlerTests : BaseTestFixture
{
    private Mock<IInvoiceManager> _invoiceManagerMock;
    private CreateInvoiceHandler _handler;

    [SetUp]
    public void Setup()
    {
        _invoiceManagerMock = new Mock<IInvoiceManager>();
        _handler = new CreateInvoiceHandler(_invoiceManagerMock.Object);
    }

    [Test]
    public async Task CreateInvoice_ValidRequest_ReturnsInvoiceDto()
    {
        // Arrange
        var createInvoiceDto = Fixture.Build<CreateInvoiceDTO>().With(x => x.ItemIds, Fixture.CreateMany<Guid>(3).ToList()).Create();

        var generatedInvoice = Mapper.Map<Data.Models.Invoice>(createInvoiceDto);
        generatedInvoice.Id = Guid.NewGuid();
        generatedInvoice.ClientBankAccountNumber = Fixture.Create<string>();
        generatedInvoice.ClientCurrencyId = Guid.NewGuid();
        generatedInvoice.ClientCurrencyCode = Fixture.Create<string>();
        generatedInvoice.ClientCurrencyAmount = Fixture.Create<decimal>();
        generatedInvoice.CustomerCurrencyId = Guid.NewGuid();
        generatedInvoice.CustomerCurrencyCode = Fixture.Create<string>();
        generatedInvoice.CustomerCurrencyAmount = Fixture.Create<decimal>();
        generatedInvoice.PaymentStatus = Data.Enums.PaymentStatus.Unpaid;

        var generatedInvoiceDto = Mapper.Map<InvoiceDTO>(generatedInvoice);

        _invoiceManagerMock
                .Setup(m => m.CreateInvoiceAsync(createInvoiceDto, CancellationToken.None))
                .ReturnsAsync(generatedInvoiceDto);

        // Act
        var result = await _handler.Handle(new CreateInvoiceRequest(createInvoiceDto), CancellationToken.None);

        // Assert
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.InstanceOf<InvoiceDTO>(), "Returned object should be an instance of InvoiceDTO");

        Assert.That(result.Value.Id, Is.EqualTo(generatedInvoice.Id));
        Assert.That(result.Value.ClientId, Is.EqualTo(createInvoiceDto.ClientId));
        Assert.That(result.Value.CustomerId, Is.EqualTo(createInvoiceDto.CustomerId));
        Assert.That(result.Value.InvoiceNumber, Is.EqualTo(createInvoiceDto.InvoiceNumber));
        Assert.That(result.Value.IssueDate, Is.EqualTo(createInvoiceDto.IssueDate));
        Assert.That(result.Value.DueDate, Is.EqualTo(createInvoiceDto.DueDate));
        Assert.That(result.Value.ItemIds, Is.EquivalentTo(createInvoiceDto.ItemIds));
        Assert.That(result.Value.ClientBankAccountNumber, Is.EqualTo(generatedInvoice.ClientBankAccountNumber));
        Assert.That(result.Value.ClientCurrencyId, Is.EqualTo(generatedInvoice.ClientCurrencyId));
        Assert.That(result.Value.ClientCurrencyCode, Is.EqualTo(generatedInvoice.ClientCurrencyCode));
        Assert.That(result.Value.ClientCurrencyAmount, Is.EqualTo(generatedInvoice.ClientCurrencyAmount));
        Assert.That(result.Value.CustomerCurrencyId, Is.EqualTo(generatedInvoice.CustomerCurrencyId));
        Assert.That(result.Value.CustomerCurrencyCode, Is.EqualTo(generatedInvoice.CustomerCurrencyCode));
        Assert.That(result.Value.CustomerCurrencyAmount, Is.EqualTo(generatedInvoice.CustomerCurrencyAmount));
        Assert.That(result.Value.PaymentStatus, Is.EqualTo((PaymentStatus)generatedInvoice.PaymentStatus));
    }
}
