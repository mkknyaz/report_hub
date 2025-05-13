using AutoFixture;
using Exadel.ReportHub.Handlers.Managers.Common;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Client;
using Exadel.ReportHub.SDK.DTOs.Customer;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Managers;

[TestFixture]
public class CountryBasedEntityManagerTests : BaseTestFixture
{
    private Mock<ICountryRepository> _countryRepositoryMock;

    private CountryBasedEntityManager _countryBasedEntityManager;

    [SetUp]
    public void Setup()
    {
        _countryRepositoryMock = new Mock<ICountryRepository>();
        _countryBasedEntityManager = new CountryBasedEntityManager(Mapper, _countryRepositoryMock.Object);
    }

    [Test]
    public async Task GenerateEntityAsync_ClientDto_ReturnsMappedClient()
    {// Arrange
        var dto = Fixture.Create<CreateClientDTO>();

        var country = Fixture.Build<Data.Models.Country>()
            .With(x => x.Id, dto.CountryId)
            .Create();

        _countryRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), CancellationToken.None))
            .ReturnsAsync([country]);

        // Act
        var result = await _countryBasedEntityManager.GenerateEntityAsync<CreateClientDTO, Data.Models.Client>(dto, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(dto.Name));
        Assert.That(result.BankAccountNumber, Is.EqualTo(dto.BankAccountNumber));
        Assert.That(result.CountryId, Is.EqualTo(dto.CountryId));
        Assert.That(result.Country, Is.EqualTo(country.Name));
        Assert.That(result.CurrencyId, Is.EqualTo(country.CurrencyId));
        Assert.That(result.CurrencyCode, Is.EqualTo(country.CurrencyCode));
        Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));

        _countryRepositoryMock.Verify(
            x => x.GetByIdsAsync(It.Is<IEnumerable<Guid>>(ids => ids.Contains(dto.CountryId)), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GenerateEntityAsync_CustomerDto_ReturnsMappedCustomer()
    {
        // Arrange
        var dto = Fixture.Create<CreateCustomerDTO>();

        var country = Fixture.Build<Data.Models.Country>()
            .With(x => x.Id, dto.CountryId)
            .Create();

        _countryRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), CancellationToken.None))
            .ReturnsAsync([country]);

        // Act
        var result = await _countryBasedEntityManager.GenerateEntityAsync<CreateCustomerDTO, Data.Models.Customer>(dto, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(dto.Name));
        Assert.That(result.Email, Is.EqualTo(dto.Email));
        Assert.That(result.ClientId, Is.EqualTo(dto.ClientId));
        Assert.That(result.CountryId, Is.EqualTo(dto.CountryId));
        Assert.That(result.Country, Is.EqualTo(country.Name));
        Assert.That(result.CurrencyId, Is.EqualTo(country.CurrencyId));
        Assert.That(result.CurrencyCode, Is.EqualTo(country.CurrencyCode));
        Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));

        _countryRepositoryMock.Verify(
            x => x.GetByIdsAsync(It.Is<IEnumerable<Guid>>(ids => ids.Contains(dto.CountryId)), CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task GenerateEntitiesAsync_MultipleClientDtos_ReturnsMappedClients()
    {
        // Arrange
        var clientDtos = Fixture.CreateMany<CreateClientDTO>(3).ToList();

        var distinctCountryIds = clientDtos.Select(dto => dto.CountryId).Distinct().ToList();

        var countries = distinctCountryIds
            .Select(id => Fixture.Build<Data.Models.Country>().With(c => c.Id, id).Create())
            .ToList();

        _countryRepositoryMock
            .Setup(x => x.GetByIdsAsync(distinctCountryIds, CancellationToken.None))
            .ReturnsAsync(countries);

        // Act
        var results = await _countryBasedEntityManager.GenerateEntitiesAsync<CreateClientDTO, Data.Models.Client>(clientDtos, CancellationToken.None);

        // Assert
        Assert.That(results, Has.Count.EqualTo(clientDtos.Count));

        foreach (var result in results)
        {
            var expectedCountry = countries.Single(c => c.Id == result.CountryId);

            Assert.That(result.Name, Is.EqualTo(clientDtos.First(dto => dto.CountryId == result.CountryId).Name));
            Assert.That(result.BankAccountNumber, Is.EqualTo(clientDtos.First(dto => dto.CountryId == result.CountryId).BankAccountNumber));
            Assert.That(result.CountryId, Is.EqualTo(expectedCountry.Id));
            Assert.That(result.Country, Is.EqualTo(expectedCountry.Name));
            Assert.That(result.CurrencyId, Is.EqualTo(expectedCountry.CurrencyId));
            Assert.That(result.CurrencyCode, Is.EqualTo(expectedCountry.CurrencyCode));
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
        }

        _countryRepositoryMock.Verify(
            x => x.GetByIdsAsync(It.Is<IEnumerable<Guid>>(ids =>
                ids.All(id => distinctCountryIds.Contains(id)) &&
                ids.Count() == distinctCountryIds.Count),
                CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task GenerateEntitiesAsync_MultipleCustomerDtos_ReturnsMappedCustomers()
    {
        // Arrange
        var customerDtos = Fixture.CreateMany<CreateCustomerDTO>(3).ToList();

        var distinctCountryIds = customerDtos.Select(dto => dto.CountryId).Distinct().ToList();

        var countries = distinctCountryIds
            .Select(id => Fixture.Build<Data.Models.Country>().With(c => c.Id, id).Create())
            .ToList();
        _countryRepositoryMock
            .Setup(x => x.GetByIdsAsync(distinctCountryIds, CancellationToken.None))
            .ReturnsAsync(countries);

        // Act
        var results = await _countryBasedEntityManager.GenerateEntitiesAsync<CreateCustomerDTO, Data.Models.Customer>(customerDtos, CancellationToken.None);

        // Assert
        Assert.That(results, Has.Count.EqualTo(customerDtos.Count));
        foreach (var result in results)
        {
            var expectedCountry = countries.Single(c => c.Id == result.CountryId);
            Assert.That(result.Name, Is.EqualTo(customerDtos.First(dto => dto.CountryId == result.CountryId).Name));
            Assert.That(result.Email, Is.EqualTo(customerDtos.First(dto => dto.CountryId == result.CountryId).Email));
            Assert.That(result.ClientId, Is.EqualTo(customerDtos.First(dto => dto.CountryId == result.CountryId).ClientId));
            Assert.That(result.CountryId, Is.EqualTo(expectedCountry.Id));
            Assert.That(result.Country, Is.EqualTo(expectedCountry.Name));
            Assert.That(result.CurrencyId, Is.EqualTo(expectedCountry.CurrencyId));
            Assert.That(result.CurrencyCode, Is.EqualTo(expectedCountry.CurrencyCode));
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
        }

        _countryRepositoryMock.Verify(
            x => x.GetByIdsAsync(It.Is<IEnumerable<Guid>>(ids =>
                ids.All(id => distinctCountryIds.Contains(id)) &&
                ids.Count() == distinctCountryIds.Count),
                CancellationToken.None),
            Times.Once);
    }
}
