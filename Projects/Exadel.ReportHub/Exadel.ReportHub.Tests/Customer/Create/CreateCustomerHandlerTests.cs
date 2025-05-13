using AutoFixture;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.Handlers.Customer.Create;
using Exadel.ReportHub.Handlers.Managers.Common;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Customer.Create;

[TestFixture]
public class CreateCustomerHandlerTests : BaseTestFixture
{
    private Mock<ICustomerRepository> _customerRepository;
    private Mock<ICountryBasedEntityManager> _countryBasedEntityManagerMock;
    private CreateCustomerHandler _handler;

    [SetUp]
    public void Setup()
    {
        _customerRepository = new Mock<ICustomerRepository>();
        _countryBasedEntityManagerMock = new Mock<ICountryBasedEntityManager>();
        _handler = new CreateCustomerHandler(_customerRepository.Object, Mapper, _countryBasedEntityManagerMock.Object);
    }

    [Test]
    public async Task CreateCustomer_ValidRequest_ReturnsCustomerDto()
    {
        // Arrange
        var createCustomerDto = Fixture.Create<CreateCustomerDTO>();
        var country = Fixture.Build<Country>().With(x => x.Id, createCustomerDto.CountryId).Create();

        var customer = Fixture.Build<Data.Models.Customer>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.Name, createCustomerDto.Name)
                .With(x => x.Email, createCustomerDto.Email)
                .With(x => x.CountryId, createCustomerDto.CountryId)
                .With(x => x.Country, country.Name)
                .With(x => x.CurrencyId, country.CurrencyId)
                .With(x => x.CurrencyCode, country.CurrencyCode)
                .Create();

        _countryBasedEntityManagerMock
            .Setup(x => x.GenerateEntityAsync<CreateCustomerDTO, Data.Models.Customer>(createCustomerDto, CancellationToken.None))
            .ReturnsAsync(customer);

        // Act
        var request = new CreateCustomerRequest(createCustomerDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.InstanceOf<CustomerDTO>(), "Returned object should be an instance of CustomerDTO");
        Assert.That(result.Value.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(result.Value.Name, Is.EqualTo(createCustomerDto.Name));
        Assert.That(result.Value.Email, Is.EqualTo(createCustomerDto.Email));
        Assert.That(result.Value.CountryId, Is.EqualTo(createCustomerDto.CountryId));
        Assert.That(result.Value.Country, Is.EqualTo(customer.Country));
        Assert.That(result.Value.CurrencyCode, Is.EqualTo(customer.CurrencyCode));
        Assert.That(result.Value.CurrencyId, Is.EqualTo(customer.CurrencyId));
        Assert.That(createCustomerDto.CountryId, Is.EqualTo(country.Id));

        _customerRepository.Verify(
            mock => mock.AddAsync(
                It.Is<Data.Models.Customer>(
                    c => c.Name == createCustomerDto.Name &&
                    c.Email == createCustomerDto.Email &&
                    c.CountryId == createCustomerDto.CountryId &&
                    c.Country == customer.Country &&
                    c.CurrencyCode == customer.CurrencyCode &&
                    c.CurrencyId == customer.CurrencyId),
                CancellationToken.None),
            Times.Once);

        _countryBasedEntityManagerMock.Verify(
            x => x.GenerateEntityAsync<CreateCustomerDTO, Data.Models.Customer>(createCustomerDto, CancellationToken.None),
            Times.Once);
    }
}
