using AutoFixture;
using ErrorOr;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.Handlers.Customer.Update;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Customer.Update;

[TestFixture]
public class UpdateCustomerHandlerTests : BaseTestFixture
{
    private UpdateCustomerHandler _handler;
    private Mock<ICustomerRepository> _customerRepositoryMock;
    private Mock<ICountryRepository> _countryRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _countryRepositoryMock = new Mock<ICountryRepository>();
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _handler = new UpdateCustomerHandler(_customerRepositoryMock.Object, _countryRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task UpdateCustomer_ValidRequest_ReturnsUpdated()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        var updateDto = Fixture.Create<UpdateCustomerDTO>();
        var country = Fixture.Build<Data.Models.Country>().With(x => x.Id, updateDto.CountryId).Create();

        _countryRepositoryMock
            .Setup(x => x.GetByIdAsync(updateDto.CountryId, CancellationToken.None))
            .ReturnsAsync(country);

        _customerRepositoryMock
            .Setup(x => x.ExistsOnClientAsync(customerId, clientId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var request = new UpdateCustomerRequest(customerId, clientId, updateDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.EqualTo(Result.Updated));
        Assert.That(country.Id, Is.EqualTo(updateDto.CountryId));

        _customerRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.Is<Data.Models.Customer>(i =>
                    i.Name == updateDto.Name &&
                    i.CountryId == updateDto.CountryId &&
                    i.Country == country.Name &&
                    i.CurrencyId == country.CurrencyId &&
                    i.CurrencyCode == country.CurrencyCode),
                CancellationToken.None),
            Times.Once);

        _customerRepositoryMock.Verify(
            x => x.ExistsOnClientAsync(customerId, clientId, CancellationToken.None),
            Times.Once);

        _countryRepositoryMock.Verify(
            x => x.GetByIdAsync(updateDto.CountryId, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task UpdateCustomer_CustomerNotFound_ReturnsNotFound()
    {
        // Arrange
        var customer = Fixture.Create<Data.Models.Customer>();
        var updateDto = Fixture.Create<UpdateCustomerDTO>();

        _customerRepositoryMock
            .Setup(x => x.UpdateAsync(customer, CancellationToken.None));
        _customerRepositoryMock
            .Setup(x => x.ExistsOnClientAsync(customer.Id, customer.ClientId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var request = new UpdateCustomerRequest(customer.Id, customer.ClientId, updateDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _customerRepositoryMock.Verify(
            x => x.ExistsOnClientAsync(customer.Id, customer.ClientId, CancellationToken.None),
            Times.Once);

        _customerRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Data.Models.Customer>(), CancellationToken.None),
            Times.Never);
    }
}
