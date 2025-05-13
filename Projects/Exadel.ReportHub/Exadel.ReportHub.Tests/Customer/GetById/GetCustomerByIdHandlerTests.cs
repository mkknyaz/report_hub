using AutoFixture;
using ErrorOr;
using Exadel.ReportHub.Handlers.Customer.GetById;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Customer.GetById;

[TestFixture]
public class GetCustomerByIdHandlerTests : BaseTestFixture
{
    private GetCustomerByIdHandler _handler;
    private Mock<ICustomerRepository> _customerRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _handler = new GetCustomerByIdHandler(_customerRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task GetCustomerById_ValidRequest_CustomerReturned()
    {
        // Arrange
        var customer = Fixture.Create<Data.Models.Customer>();

        _customerRepositoryMock
            .Setup(x => x.GetByIdAsync(customer.Id, CancellationToken.None))
            .ReturnsAsync(customer);

        // Act
        var request = new GetCustomerByIdRequest(customer.Id, customer.ClientId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Not.Null);

        Assert.That(result.Value.Id, Is.EqualTo(customer.Id));
        Assert.That(result.Value.Email, Is.EqualTo(customer.Email));
        Assert.That(result.Value.Name, Is.EqualTo(customer.Name));
        Assert.That(result.Value.Country, Is.EqualTo(customer.Country));
        Assert.That(result.Value.CountryId, Is.EqualTo(customer.CountryId));
        Assert.That(result.Value.CurrencyId, Is.EqualTo(customer.CurrencyId));
        Assert.That(result.Value.CurrencyCode, Is.EqualTo(customer.CurrencyCode));

        _customerRepositoryMock.Verify(
            x => x.GetByIdAsync(customer.Id, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task GetCustomerById_CustomerNotFound_ReturnsNotFound()
    {
        // Arrange
        var customer = Fixture.Create<Data.Models.Customer>();

        // Act
        var request = new GetCustomerByIdRequest(customer.Id, customer.ClientId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.Errors, Has.Count.EqualTo(1), "Should contains the only error");
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _customerRepositoryMock.Verify(
            repo => repo.GetByIdAsync(customer.Id, CancellationToken.None),
            Times.Once);
    }
}
