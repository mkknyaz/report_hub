using AutoFixture;
using Exadel.ReportHub.Data.Enums;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.Ecb;
using Exadel.ReportHub.Ecb.Abstract;
using Exadel.ReportHub.Handlers.Managers;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Managers;

[TestFixture]
public class InvoiceManagerTests : BaseTestFixture
{
    private Mock<ICustomerRepository> _customerRepositoryMock;
    private Mock<IItemRepository> _itemRepositoryMock;
    private Mock<ICurrencyConverter> _currencyConverterMock;
    private Mock<IClientRepository> _clientRepositoryMock;

    private InvoiceManager _invoiceManager;

    [SetUp]
    public void Setup()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _itemRepositoryMock = new Mock<IItemRepository>();
        _currencyConverterMock = new Mock<ICurrencyConverter>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _invoiceManager = new InvoiceManager(
            _clientRepositoryMock.Object,
            _customerRepositoryMock.Object,
            _itemRepositoryMock.Object,
            _currencyConverterMock.Object,
            Mapper);
    }

    [Test]
    public async Task GenerateInvoiceAsync_ReturnsInvoice()
    {
        // Arrange
        var customer = Fixture.Build<Customer>()
            .With(x => x.CurrencyCode, Constants.Currency.DefaultCurrencyCode)
            .Create();
        var client = Fixture.Build<Client>()
            .With(x => x.CurrencyCode, Constants.Currency.DefaultCurrencyCode)
            .Create();
        var items = Fixture.Build<Data.Models.Item>()
            .With(x => x.CurrencyCode, Constants.Currency.DefaultCurrencyCode)
            .CreateMany(3).ToList();
        var itemIds = items.Select(x => x.Id).ToList();
        var invoiceDto = Fixture.Build<CreateInvoiceDTO>()
            .With(x => x.CustomerId, customer.Id)
            .With(x => x.ItemIds, itemIds)
            .With(x => x.ClientId, client.Id)
            .Create();
        var amount = items.Sum(x => x.Price);

        _customerRepositoryMock
            .Setup(x => x.GetByIdsAsync(new[] { customer.Id }, CancellationToken.None))
            .ReturnsAsync(new List<Customer> { customer });

        _itemRepositoryMock
            .Setup(x => x.GetByIdsAsync(invoiceDto.ItemIds, CancellationToken.None))
            .ReturnsAsync(items);

        _currencyConverterMock
            .Setup(x => x.ConvertAsync(amount, Constants.Currency.DefaultCurrencyCode,
                Constants.Currency.DefaultCurrencyCode, invoiceDto.IssueDate, CancellationToken.None))
            .ReturnsAsync(amount);

        _clientRepositoryMock
            .Setup(x => x.GetByIdsAsync(new[] { client.Id }, CancellationToken.None))
            .ReturnsAsync(new List<Client> { client });

        // Act
        var result = await _invoiceManager.GenerateInvoiceAsync(invoiceDto, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ClientId, Is.EqualTo(invoiceDto.ClientId));
        Assert.That(result.CustomerId, Is.EqualTo(invoiceDto.CustomerId));
        Assert.That(result.InvoiceNumber, Is.EqualTo(invoiceDto.InvoiceNumber));
        Assert.That(result.IssueDate, Is.EqualTo(invoiceDto.IssueDate));
        Assert.That(result.DueDate, Is.EqualTo(invoiceDto.DueDate));
        Assert.That(result.ClientBankAccountNumber, Is.EqualTo(client.BankAccountNumber));
        Assert.That(result.CustomerCurrencyAmount, Is.EqualTo(amount));
        Assert.That(result.CustomerCurrencyId, Is.EqualTo(customer.CurrencyId));
        Assert.That(result.CustomerCurrencyCode, Is.EqualTo(customer.CurrencyCode));
        Assert.That(result.ClientCurrencyAmount, Is.EqualTo(amount));
        Assert.That(result.ClientCurrencyId, Is.EqualTo(client.CurrencyId));
        Assert.That(result.ClientCurrencyCode, Is.EqualTo(client.CurrencyCode));
        Assert.That(result.PaymentStatus, Is.EqualTo(PaymentStatus.Unpaid));
        Assert.That(result.ItemIds, Is.EqualTo(invoiceDto.ItemIds));
    }

    [Test]
    public async Task GenerateInvoicesAsync_ReturnsInvoices()
    {
        // Arrange
        var customers = Fixture.Build<Customer>()
            .With(x => x.CurrencyCode, Constants.Currency.DefaultCurrencyCode)
            .CreateMany(1).ToList();
        var customerId = customers[0].Id;
        var clients = Fixture.Build<Client>()
            .With(x => x.CurrencyCode, Constants.Currency.DefaultCurrencyCode)
            .CreateMany(1).ToList();
        var clientId = clients[0].Id;
        var items = Fixture.Build<Data.Models.Item>()
            .With(x => x.CurrencyCode, Constants.Currency.DefaultCurrencyCode)
            .CreateMany(3).ToList();
        var itemIds = items.Select(x => x.Id).ToList();
        var date = DateTime.Now;
        var invoiceDtos = Fixture.Build<CreateInvoiceDTO>()
            .With(x => x.CustomerId, customers[0].Id)
            .With(x => x.ItemIds, itemIds)
            .With(x => x.IssueDate, date)
            .With(x => x.ClientId, clients[0].Id)
            .CreateMany(2).ToList();
        var amount = items.Sum(x => x.Price);

        _customerRepositoryMock
            .Setup(x => x.GetByIdsAsync(new[] { customerId }, CancellationToken.None))
            .ReturnsAsync(customers);

        _itemRepositoryMock
            .Setup(x => x.GetByIdsAsync(itemIds, CancellationToken.None))
            .ReturnsAsync(items);

        _currencyConverterMock
            .Setup(x => x.ConvertAsync(amount, Constants.Currency.DefaultCurrencyCode,
                Constants.Currency.DefaultCurrencyCode, date, CancellationToken.None))
            .ReturnsAsync(amount);

        _clientRepositoryMock.Setup(x => x.GetByIdsAsync(new[] { clientId }, CancellationToken.None))
            .ReturnsAsync(clients);

        // Act
        var result = await _invoiceManager.GenerateInvoicesAsync(invoiceDtos, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Empty);
        Assert.That(result.Count, Is.EqualTo(invoiceDtos.Count));

        Assert.That(result[0].ClientId, Is.EqualTo(clientId));
        Assert.That(result[0].CustomerId, Is.EqualTo(customerId));
        Assert.That(result[0].InvoiceNumber, Is.EqualTo(invoiceDtos[0].InvoiceNumber));
        Assert.That(result[0].IssueDate, Is.EqualTo(invoiceDtos[0].IssueDate));
        Assert.That(result[0].DueDate, Is.EqualTo(invoiceDtos[0].DueDate));
        Assert.That(result[0].ClientBankAccountNumber, Is.EqualTo(clients[0].BankAccountNumber));
        Assert.That(result[0].CustomerCurrencyAmount, Is.EqualTo(amount));
        Assert.That(result[0].CustomerCurrencyId, Is.EqualTo(customers[0].CurrencyId));
        Assert.That(result[0].CustomerCurrencyCode, Is.EqualTo(customers[0].CurrencyCode));
        Assert.That(result[0].ClientCurrencyAmount, Is.EqualTo(amount));
        Assert.That(result[0].ClientCurrencyId, Is.EqualTo(clients[0].CurrencyId));
        Assert.That(result[0].ClientCurrencyCode, Is.EqualTo(clients[0].CurrencyCode));
        Assert.That(result[0].PaymentStatus, Is.EqualTo(PaymentStatus.Unpaid));
        Assert.That(result[0].ItemIds, Is.EqualTo(itemIds));

        Assert.That(result[1].ClientId, Is.EqualTo(clientId));
        Assert.That(result[1].CustomerId, Is.EqualTo(customerId));
        Assert.That(result[1].InvoiceNumber, Is.EqualTo(invoiceDtos[1].InvoiceNumber));
        Assert.That(result[1].IssueDate, Is.EqualTo(invoiceDtos[1].IssueDate));
        Assert.That(result[1].DueDate, Is.EqualTo(invoiceDtos[1].DueDate));
        Assert.That(result[1].ClientBankAccountNumber, Is.EqualTo(clients[0].BankAccountNumber));
        Assert.That(result[1].CustomerCurrencyAmount, Is.EqualTo(amount));
        Assert.That(result[1].CustomerCurrencyId, Is.EqualTo(customers[0].CurrencyId));
        Assert.That(result[1].CustomerCurrencyCode, Is.EqualTo(customers[0].CurrencyCode));
        Assert.That(result[1].ClientCurrencyAmount, Is.EqualTo(amount));
        Assert.That(result[1].ClientCurrencyId, Is.EqualTo(clients[0].CurrencyId));
        Assert.That(result[1].ClientCurrencyCode, Is.EqualTo(clients[0].CurrencyCode));
        Assert.That(result[1].PaymentStatus, Is.EqualTo(PaymentStatus.Unpaid));
        Assert.That(result[1].ItemIds, Is.EqualTo(itemIds));
    }
}
