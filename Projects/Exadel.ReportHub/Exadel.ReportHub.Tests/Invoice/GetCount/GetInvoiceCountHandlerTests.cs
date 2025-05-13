using AutoFixture;
using Exadel.ReportHub.Handlers.Invoice.GetCount;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Invoice;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Invoice.GetCount;

[TestFixture]
public class GetInvoiceCountHandlerTests : BaseTestFixture
{
    private Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private GetInvoiceCountHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _handler = new GetInvoiceCountHandler(_invoiceRepositoryMock.Object);
    }

    [Test]
    public async Task GetInvoiceCount_WhenExist_ReturnsInvoiceCountResultDTO()
    {
        // Arrange
        var startDate = Fixture.Create<DateTime>();
        var endDate = startDate.AddDays(10);
        var clientId = Guid.NewGuid();
        var customer1 = Guid.NewGuid();
        var customer2 = Guid.NewGuid();

        var filterDto = new InvoiceCountFilterDTO
        {
            StartDate = startDate,
            EndDate = endDate,
            ClientId = clientId,
            CustomerId = null
        };

        var dictionaryResult = new Dictionary<Guid, int>
        {
            [customer1] = 5,
            [customer2] = 3
        };

        _invoiceRepositoryMock
            .Setup(r => r.GetCountByDateRangeAsync(startDate, endDate, clientId, filterDto.CustomerId, CancellationToken.None))
            .ReturnsAsync(dictionaryResult);

        // Act
        var result = await _handler.Handle(new GetInvoiceCountRequest(filterDto), CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value.Total, Is.EqualTo(dictionaryResult.Sum(x => x.Value)));
        Assert.That(result.Value.Customers, Is.EqualTo(dictionaryResult));

        _invoiceRepositoryMock.Verify(r =>
            r.GetCountByDateRangeAsync(
                startDate, endDate, clientId, filterDto.CustomerId, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task GetInvoiceCount_WhenNoExist_ReturnsEmpty()
    {
        // Arrange
        var startDate = Fixture.Create<DateTime>();
        var endDate = startDate.AddDays(10);
        var clientId = Guid.NewGuid();
        var filterDto = new InvoiceCountFilterDTO
        {
            StartDate = startDate,
            EndDate = endDate,
            ClientId = clientId,
            CustomerId = null
        };

        _invoiceRepositoryMock
            .Setup(r => r.GetCountByDateRangeAsync(startDate, endDate, clientId, filterDto.CustomerId, CancellationToken.None))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(new GetInvoiceCountRequest(filterDto), CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value.Total, Is.EqualTo(0));
        Assert.That(result.Value.Customers, Is.Empty);

        _invoiceRepositoryMock.Verify(r =>
            r.GetCountByDateRangeAsync(
                startDate, endDate, clientId, filterDto.CustomerId, CancellationToken.None),
            Times.Once);
    }
}
