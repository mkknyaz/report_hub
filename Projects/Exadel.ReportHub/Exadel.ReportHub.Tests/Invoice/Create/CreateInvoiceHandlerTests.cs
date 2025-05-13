using AutoFixture;
using Exadel.ReportHub.Handlers.Invoice.Create;
using Exadel.ReportHub.Handlers.Managers.Invoice;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Invoice.Create;

[TestFixture]
public class CreateInvoiceHandlerTests : BaseTestFixture
{
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private Mock<IInvoiceManager> _invoiceManagerMock;
    private CreateInvoiceHandler _handler;

    [SetUp]
    public void Setup()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _invoiceManagerMock = new Mock<IInvoiceManager>();
        _handler = new CreateInvoiceHandler(_invoiceRepositoryMock.Object, _invoiceManagerMock.Object, Mapper);
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

        _invoiceManagerMock
                .Setup(m => m.GenerateInvoiceAsync(createInvoiceDto, CancellationToken.None))
                .ReturnsAsync(generatedInvoice);

        // Act
        var result = await _handler.Handle(new CreateInvoiceRequest(createInvoiceDto), CancellationToken.None);

        // Assert
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.InstanceOf<InvoiceDTO>(), "Returned object should be an instance of InvoiceDTO");

        _invoiceRepositoryMock.Verify(
            mock => mock.AddAsync(
                It.Is<Data.Models.Invoice>(
                    i => i.ClientId == createInvoiceDto.ClientId &&
                    i.CustomerId == createInvoiceDto.CustomerId &&
                    i.InvoiceNumber == createInvoiceDto.InvoiceNumber &&
                    i.IssueDate == createInvoiceDto.IssueDate &&
                    i.DueDate == createInvoiceDto.DueDate &&
                    i.ItemIds.SequenceEqual(createInvoiceDto.ItemIds) &&
                    i.ClientBankAccountNumber == generatedInvoice.ClientBankAccountNumber &&
                    i.ClientCurrencyId == generatedInvoice.ClientCurrencyId &&
                    i.ClientCurrencyCode == generatedInvoice.ClientCurrencyCode &&
                    i.ClientCurrencyAmount == generatedInvoice.ClientCurrencyAmount &&
                    i.CustomerCurrencyId == generatedInvoice.CustomerCurrencyId &&
                    i.CustomerCurrencyCode == generatedInvoice.CustomerCurrencyCode &&
                    i.CustomerCurrencyAmount == generatedInvoice.CustomerCurrencyAmount &&
                    i.PaymentStatus == generatedInvoice.PaymentStatus),
                CancellationToken.None),
            Times.Once);
    }
}
