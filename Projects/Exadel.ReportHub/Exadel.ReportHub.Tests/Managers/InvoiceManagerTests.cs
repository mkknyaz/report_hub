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

    private InvoiceManager _invoiceManager;

    [SetUp]
    public void Setup()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _itemRepositoryMock = new Mock<IItemRepository>();
        _currencyConverterMock = new Mock<ICurrencyConverter>();

        _invoiceManager = new InvoiceManager(
            _customerRepositoryMock.Object,
            _itemRepositoryMock.Object,
            _currencyConverterMock.Object,
            Mapper);
    }

    [Test]
    public async Task GenerateInvoiceAsync_ReturnsInvoice()
    {
        // Arrange
        var invoiceDto = Fixture.Create<CreateInvoiceDTO>();
        var customer = Fixture.Build<Customer>()
            .With(x => x.Id, invoiceDto.CustomerId)
            .With(x => x.CurrencyCode, Constants.Currency.DefaultCurrencyCode)
            .Create();
        var items = invoiceDto.ItemIds.Select(id =>
            Fixture.Build<Item>()
            .With(x => x.Id, id)
            .With(x => x.CurrencyCode, Constants.Currency.DefaultCurrencyCode)
            .Create()).ToList();
        var amount = items.Sum(x => x.Price);

        _customerRepositoryMock
            .Setup(x => x.GetByIdsAsync(new[] { customer.Id }, CancellationToken.None))
            .ReturnsAsync(new List<Customer> { customer });

        _itemRepositoryMock
            .Setup(x => x.GetByIdsAsync(invoiceDto.ItemIds, CancellationToken.None))
            .ReturnsAsync(items);

        _currencyConverterMock
            .Setup(x => x.ConvertAsync(amount, Constants.Currency.DefaultCurrencyCode,
                Constants.Currency.DefaultCurrencyCode, CancellationToken.None))
            .ReturnsAsync(amount);

        // Act
        var result = await _invoiceManager.GenerateInvoiceAsync(invoiceDto, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ClientId, Is.EqualTo(invoiceDto.ClientId));
        Assert.That(result.CustomerId, Is.EqualTo(invoiceDto.CustomerId));
        Assert.That(result.InvoiceNumber, Is.EqualTo(invoiceDto.InvoiceNumber));
        Assert.That(result.IssueDate, Is.EqualTo(invoiceDto.IssueDate));
        Assert.That(result.DueDate, Is.EqualTo(invoiceDto.DueDate));
        Assert.That(result.Amount, Is.EqualTo(amount));
        Assert.That(result.CurrencyId, Is.EqualTo(customer.CurrencyId));
        Assert.That(result.CurrencyCode, Is.EqualTo(customer.CurrencyCode));
        Assert.That(result.PaymentStatus, Is.EqualTo((PaymentStatus)invoiceDto.PaymentStatus));
        Assert.That(result.ItemIds, Is.EqualTo(invoiceDto.ItemIds));
    }

    [Test]
    public async Task GenerateInvoicesAsync_ReturnsInvoices()
    {
        // Arrange
        var customers = Fixture.Build<Customer>()
            .With(x => x.CurrencyCode, Constants.Currency.DefaultCurrencyCode)
            .CreateMany(1).ToList();
        var custimerId = customers[0].Id;
        var items = Fixture.Build<Item>()
            .With(x => x.CurrencyCode, Constants.Currency.DefaultCurrencyCode)
            .CreateMany(3).ToList();
        var itemIds = items.Select(x => x.Id).ToList();
        var invoiceDtos = Fixture.Build<CreateInvoiceDTO>()
            .With(x => x.CustomerId, customers[0].Id)
            .With(x => x.ItemIds, itemIds)
            .CreateMany(2).ToList();
        var amount = items.Sum(x => x.Price);

        _customerRepositoryMock
            .Setup(x => x.GetByIdsAsync(new[] { custimerId }, CancellationToken.None))
            .ReturnsAsync(customers);

        _itemRepositoryMock
            .Setup(x => x.GetByIdsAsync(itemIds, CancellationToken.None))
            .ReturnsAsync(items);

        _currencyConverterMock
            .Setup(x => x.ConvertAsync(amount, Constants.Currency.DefaultCurrencyCode,
                Constants.Currency.DefaultCurrencyCode, CancellationToken.None))
            .ReturnsAsync(amount);

        // Act
        var result = await _invoiceManager.GenerateInvoicesAsync(invoiceDtos, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Empty);
        Assert.That(result.Count, Is.EqualTo(invoiceDtos.Count));

        Assert.That(result[0].ClientId, Is.EqualTo(invoiceDtos[0].ClientId));
        Assert.That(result[0].CustomerId, Is.EqualTo(custimerId));
        Assert.That(result[0].InvoiceNumber, Is.EqualTo(invoiceDtos[0].InvoiceNumber));
        Assert.That(result[0].IssueDate, Is.EqualTo(invoiceDtos[0].IssueDate));
        Assert.That(result[0].DueDate, Is.EqualTo(invoiceDtos[0].DueDate));
        Assert.That(result[0].Amount, Is.EqualTo(amount));
        Assert.That(result[0].CurrencyId, Is.EqualTo(customers[0].CurrencyId));
        Assert.That(result[0].CurrencyCode, Is.EqualTo(customers[0].CurrencyCode));
        Assert.That(result[0].PaymentStatus, Is.EqualTo((PaymentStatus)invoiceDtos[0].PaymentStatus));
        Assert.That(result[0].ItemIds, Is.EqualTo(itemIds));

        Assert.That(result[1].ClientId, Is.EqualTo(invoiceDtos[1].ClientId));
        Assert.That(result[1].CustomerId, Is.EqualTo(custimerId));
        Assert.That(result[1].InvoiceNumber, Is.EqualTo(invoiceDtos[1].InvoiceNumber));
        Assert.That(result[1].IssueDate, Is.EqualTo(invoiceDtos[1].IssueDate));
        Assert.That(result[1].DueDate, Is.EqualTo(invoiceDtos[1].DueDate));
        Assert.That(result[1].Amount, Is.EqualTo(amount));
        Assert.That(result[1].CurrencyId, Is.EqualTo(customers[0].CurrencyId));
        Assert.That(result[1].CurrencyCode, Is.EqualTo(customers[0].CurrencyCode));
        Assert.That(result[1].PaymentStatus, Is.EqualTo((PaymentStatus)invoiceDtos[1].PaymentStatus));
        Assert.That(result[1].ItemIds, Is.EqualTo(itemIds));
    }
}
