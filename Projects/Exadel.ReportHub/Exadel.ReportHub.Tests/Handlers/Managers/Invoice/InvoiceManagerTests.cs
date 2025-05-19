using AutoFixture;
using Exadel.ReportHub.Ecb;
using Exadel.ReportHub.Ecb.Abstract;
using Exadel.ReportHub.Handlers.Managers.Invoice;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using Exadel.ReportHub.SDK.Enums;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Managers.Invoice;

[TestFixture]
public class InvoiceManagerTests : BaseTestFixture
{
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private Mock<IClientRepository> _clientRepositoryMock;
    private Mock<IItemRepository> _itemRepositoryMock;
    private Mock<ICustomerRepository> _customerRepositoryMock;
    private Mock<ICurrencyConverter> _currencyConverterMock;

    private InvoiceManager _invoiceManager;

    [SetUp]
    public void Setup()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _itemRepositoryMock = new Mock<IItemRepository>();
        _currencyConverterMock = new Mock<ICurrencyConverter>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _invoiceManager = new InvoiceManager(
            _invoiceRepositoryMock.Object,
            _clientRepositoryMock.Object,
            _customerRepositoryMock.Object,
            _itemRepositoryMock.Object,
            _currencyConverterMock.Object,
            Mapper);
    }

    [Test]
    public async Task GenerateInvoiceAsync_ReturnsInvoiceDto()
    {
        // Arrange
        var customer = Fixture.Build<Data.Models.Customer>()
            .With(x => x.CurrencyCode, Constants.Currency.DefaultCurrencyCode)
            .Create();
        var client = Fixture.Build<Data.Models.Client>()
            .With(x => x.CurrencyCode, Constants.Currency.DefaultCurrencyCode)
            .Create();
        var items = Fixture.Build<Data.Models.Item>()
            .With(x => x.CurrencyCode, Constants.Currency.DefaultCurrencyCode)
            .CreateMany(3).ToList();
        var itemIds = items.Select(x => x.Id).ToList();
        var createInvoiceDto = Fixture.Build<CreateInvoiceDTO>()
            .With(x => x.CustomerId, customer.Id)
            .With(x => x.ItemIds, itemIds)
            .With(x => x.ClientId, client.Id)
            .Create();
        var amount = items.Sum(x => x.Price);

        _customerRepositoryMock
            .Setup(x => x.GetByIdsAsync(new[] { customer.Id }, CancellationToken.None))
            .ReturnsAsync(new List<Data.Models.Customer> { customer });

        _itemRepositoryMock
            .Setup(x => x.GetByIdsAsync(createInvoiceDto.ItemIds, CancellationToken.None))
            .ReturnsAsync(items);

        _currencyConverterMock
            .Setup(x => x.ConvertAsync(amount, Constants.Currency.DefaultCurrencyCode,
                Constants.Currency.DefaultCurrencyCode, createInvoiceDto.IssueDate, CancellationToken.None))
            .ReturnsAsync(amount);

        _clientRepositoryMock
            .Setup(x => x.GetByIdsAsync(new[] { client.Id }, CancellationToken.None))
            .ReturnsAsync(new List<Data.Models.Client> { client });

        // Act
        var result = await _invoiceManager.CreateInvoiceAsync(createInvoiceDto, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);

        AssertInvoiceDto(result, createInvoiceDto, client, customer, amount);

        _invoiceRepositoryMock.Verify(
            repo => repo.AddManyAsync(
                It.Is<IList<Data.Models.Invoice>>(
                    inv => inv.Count() == 1 &&
                        inv[0].ClientId == createInvoiceDto.ClientId &&
                        inv[0].CustomerId == createInvoiceDto.CustomerId &&
                        inv[0].InvoiceNumber == createInvoiceDto.InvoiceNumber &&
                        inv[0].IssueDate == createInvoiceDto.IssueDate &&
                        inv[0].DueDate == createInvoiceDto.DueDate &&
                        inv[0].ItemIds.SequenceEqual(createInvoiceDto.ItemIds) &&
                        inv[0].ClientBankAccountNumber == client.BankAccountNumber &&
                        inv[0].ClientCurrencyId == client.CurrencyId &&
                        inv[0].ClientCurrencyCode == client.CurrencyCode &&
                        inv[0].ClientCurrencyAmount == amount &&
                        inv[0].CustomerCurrencyId == customer.CurrencyId &&
                        inv[0].CustomerCurrencyCode == customer.CurrencyCode &&
                        inv[0].CustomerCurrencyAmount == amount &&
                        inv[0].PaymentStatus == (Data.Enums.PaymentStatus)PaymentStatus.Unpaid),
                CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task GenerateInvoicesAsync_ReturnsInvoices()
    {
        // Arrange
        var customers = Fixture.Build<Data.Models.Customer>()
            .With(x => x.CurrencyCode, Constants.Currency.DefaultCurrencyCode)
            .CreateMany(1).ToList();
        var clients = Fixture.Build<Data.Models.Client>()
            .With(x => x.CurrencyCode, Constants.Currency.DefaultCurrencyCode)
            .CreateMany(1).ToList();
        var items = Fixture.Build<Data.Models.Item>()
            .With(x => x.CurrencyCode, Constants.Currency.DefaultCurrencyCode)
            .CreateMany(3).ToList();
        var itemIds = items.Select(x => x.Id).ToList();
        var date = DateTime.Now;
        var createInvoiceDtos = Fixture.Build<CreateInvoiceDTO>()
            .With(x => x.CustomerId, customers[0].Id)
            .With(x => x.ItemIds, itemIds)
            .With(x => x.IssueDate, date)
            .With(x => x.ClientId, clients[0].Id)
            .CreateMany(2).ToList();
        var amount = items.Sum(x => x.Price);

        _customerRepositoryMock
            .Setup(x => x.GetByIdsAsync(new[] { customers[0].Id }, CancellationToken.None))
            .ReturnsAsync(customers);

        _itemRepositoryMock
            .Setup(x => x.GetByIdsAsync(itemIds, CancellationToken.None))
            .ReturnsAsync(items);

        _currencyConverterMock
            .Setup(x => x.ConvertAsync(amount, Constants.Currency.DefaultCurrencyCode,
                Constants.Currency.DefaultCurrencyCode, date, CancellationToken.None))
            .ReturnsAsync(amount);

        _clientRepositoryMock.Setup(x => x.GetByIdsAsync(new[] { clients[0].Id }, CancellationToken.None))
            .ReturnsAsync(clients);

        // Act
        var result = await _invoiceManager.CreateInvoicesAsync(createInvoiceDtos, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Empty);
        Assert.That(result.Count, Is.EqualTo(createInvoiceDtos.Count));

        AssertInvoiceDto(result[0], createInvoiceDtos[0], clients[0], customers[0], amount);
        AssertInvoiceDto(result[1], createInvoiceDtos[1], clients[0], customers[0], amount);

        _invoiceRepositoryMock.Verify(
            repo => repo.AddManyAsync(
                It.Is<IList<Data.Models.Invoice>>(
                    inv => inv.Count() == 2 &&
                        inv[0].ClientId == createInvoiceDtos[0].ClientId &&
                        inv[0].CustomerId == createInvoiceDtos[0].CustomerId &&
                        inv[0].InvoiceNumber == createInvoiceDtos[0].InvoiceNumber &&
                        inv[0].IssueDate == createInvoiceDtos[0].IssueDate &&
                        inv[0].DueDate == createInvoiceDtos[0].DueDate &&
                        inv[0].ItemIds.SequenceEqual(createInvoiceDtos[0].ItemIds) &&
                        inv[0].ClientBankAccountNumber == clients[0].BankAccountNumber &&
                        inv[0].ClientCurrencyId == clients[0].CurrencyId &&
                        inv[0].ClientCurrencyCode == clients[0].CurrencyCode &&
                        inv[0].ClientCurrencyAmount == amount &&
                        inv[0].CustomerCurrencyId == customers[0].CurrencyId &&
                        inv[0].CustomerCurrencyCode == customers[0].CurrencyCode &&
                        inv[0].CustomerCurrencyAmount == amount &&
                        inv[0].PaymentStatus == (Data.Enums.PaymentStatus)PaymentStatus.Unpaid &&

                        inv[1].ClientId == createInvoiceDtos[1].ClientId &&
                        inv[1].CustomerId == createInvoiceDtos[1].CustomerId &&
                        inv[1].InvoiceNumber == createInvoiceDtos[1].InvoiceNumber &&
                        inv[1].IssueDate == createInvoiceDtos[1].IssueDate &&
                        inv[1].DueDate == createInvoiceDtos[1].DueDate &&
                        inv[1].ItemIds.SequenceEqual(createInvoiceDtos[1].ItemIds) &&
                        inv[1].ClientBankAccountNumber == clients[0].BankAccountNumber &&
                        inv[1].ClientCurrencyId == clients[0].CurrencyId &&
                        inv[1].ClientCurrencyCode == clients[0].CurrencyCode &&
                        inv[1].ClientCurrencyAmount == amount &&
                        inv[1].CustomerCurrencyId == customers[0].CurrencyId &&
                        inv[1].CustomerCurrencyCode == customers[0].CurrencyCode &&
                        inv[1].CustomerCurrencyAmount == amount &&
                        inv[1].PaymentStatus == (Data.Enums.PaymentStatus)PaymentStatus.Unpaid),
                CancellationToken.None),
            Times.Once);
    }

    private static void AssertInvoiceDto(InvoiceDTO actual, CreateInvoiceDTO expectedFromCreateDto,
        Data.Models.Client expectedFromClient, Data.Models.Customer expectedFromCustomer, decimal expectedAmount)
    {
        Assert.That(actual.ClientId, Is.EqualTo(expectedFromCreateDto.ClientId));
        Assert.That(actual.CustomerId, Is.EqualTo(expectedFromCreateDto.CustomerId));
        Assert.That(actual.InvoiceNumber, Is.EqualTo(expectedFromCreateDto.InvoiceNumber));
        Assert.That(actual.IssueDate, Is.EqualTo(expectedFromCreateDto.IssueDate));
        Assert.That(actual.DueDate, Is.EqualTo(expectedFromCreateDto.DueDate));
        Assert.That(actual.ItemIds, Is.EquivalentTo(expectedFromCreateDto.ItemIds));
        Assert.That(actual.ClientBankAccountNumber, Is.EqualTo(expectedFromClient.BankAccountNumber));
        Assert.That(actual.ClientCurrencyId, Is.EqualTo(expectedFromClient.CurrencyId));
        Assert.That(actual.ClientCurrencyCode, Is.EqualTo(expectedFromClient.CurrencyCode));
        Assert.That(actual.ClientCurrencyAmount, Is.EqualTo(expectedAmount));
        Assert.That(actual.CustomerCurrencyId, Is.EqualTo(expectedFromCustomer.CurrencyId));
        Assert.That(actual.CustomerCurrencyCode, Is.EqualTo(expectedFromCustomer.CurrencyCode));
        Assert.That(actual.CustomerCurrencyAmount, Is.EqualTo(expectedAmount));
        Assert.That(actual.PaymentStatus, Is.EqualTo(PaymentStatus.Unpaid));
    }
}
