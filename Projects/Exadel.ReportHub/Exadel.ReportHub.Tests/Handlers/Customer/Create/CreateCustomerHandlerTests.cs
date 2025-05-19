using AutoFixture;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.Handlers.Customer.Create;
using Exadel.ReportHub.Handlers.Managers.Customer;
using Exadel.ReportHub.SDK.DTOs.Client;
using Exadel.ReportHub.SDK.DTOs.Customer;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Customer.Create;

[TestFixture]
public class CreateCustomerHandlerTests : BaseTestFixture
{
    private Mock<ICustomerManager> _customerManagerMock;
    private CreateCustomerHandler _handler;

    [SetUp]
    public void Setup()
    {
        _customerManagerMock = new Mock<ICustomerManager>();
        _handler = new CreateCustomerHandler(_customerManagerMock.Object);
    }

    [Test]
    public async Task CreateCustomer_ValidRequest_ReturnsCustomerDto()
    {
        // Arrange
        var createCustomerDto = Fixture.Create<CreateCustomerDTO>();
        var customerDto = Fixture.Create<CustomerDTO>();

        _customerManagerMock
            .Setup(x => x.CreateCustomerAsync(createCustomerDto, CancellationToken.None))
            .ReturnsAsync(customerDto);

        // Act
        var request = new CreateCustomerRequest(createCustomerDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.InstanceOf<CustomerDTO>(), "Returned object should be an instance of CustomerDTO");
        Assert.That(result.Value.Id, Is.EqualTo(customerDto.Id));
        Assert.That(result.Value.Name, Is.EqualTo(customerDto.Name));
        Assert.That(result.Value.Email, Is.EqualTo(customerDto.Email));
        Assert.That(result.Value.CountryId, Is.EqualTo(customerDto.CountryId));
        Assert.That(result.Value.Country, Is.EqualTo(customerDto.Country));
        Assert.That(result.Value.CurrencyCode, Is.EqualTo(customerDto.CurrencyCode));
        Assert.That(result.Value.CurrencyId, Is.EqualTo(customerDto.CurrencyId));
    }
}
