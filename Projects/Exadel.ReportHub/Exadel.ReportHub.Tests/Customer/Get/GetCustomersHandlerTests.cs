using AutoFixture;
using Exadel.ReportHub.Handlers.Customer.Get;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Customer.Get;

[TestFixture]
public class GetCustomersHandlerTests : BaseTestFixture
{
    private GetCustomersByClientIdHandler _handler;
    private Mock<ICustomerRepository> _customerRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _handler = new GetCustomersByClientIdHandler(_customerRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task GetCustomers_ClientHasCustomers_ReturnsCustomerDTOs()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var customers = Fixture.Build<Data.Models.Customer>().With(x => x.ClientId, clientId).CreateMany(2).ToList();

        _customerRepositoryMock
            .Setup(x => x.GetByClientIdAsync(clientId, CancellationToken.None))
            .ReturnsAsync(customers);

        // Act
        var request = new GetCustomersByClientIdRequest(clientId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.ToList(), Has.Count.EqualTo(customers.Count));

        for (int i = 0; i < result.Value.Count; i++)
        {
            Assert.Multiple(() =>
            {
                Assert.That(result.Value[i].Id, Is.EqualTo(customers[i].Id));
                Assert.That(result.Value[i].Name, Is.EqualTo(customers[i].Name));
                Assert.That(result.Value[i].Email, Is.EqualTo(customers[i].Email));
                Assert.That(result.Value[i].Country, Is.EqualTo(customers[i].Country));
                Assert.That(result.Value[i].CountryId, Is.EqualTo(customers[i].CountryId));
                Assert.That(result.Value[i].CurrencyId, Is.EqualTo(customers[i].CurrencyId));
                Assert.That(result.Value[i].CurrencyCode, Is.EqualTo(customers[i].CurrencyCode));
            });
        }

        _customerRepositoryMock.Verify(
            x => x.GetByClientIdAsync(clientId, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task GetCustomers_ClientHasNoCustomers_ReturnsEmptyList()
    {
        var clientId = Guid.NewGuid();

        _customerRepositoryMock
            .Setup(x => x.GetByClientIdAsync(clientId, CancellationToken.None))
            .ReturnsAsync(new List<Data.Models.Customer>());

        // Act
        var request = new GetCustomersByClientIdRequest(clientId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Empty);

        _customerRepositoryMock.Verify(
            x => x.GetByClientIdAsync(clientId, CancellationToken.None),
            Times.Once);
    }
}
