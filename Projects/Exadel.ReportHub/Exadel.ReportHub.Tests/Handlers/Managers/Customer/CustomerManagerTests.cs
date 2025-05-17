using AutoFixture;
using Exadel.ReportHub.Handlers.Managers.Customer;
using Exadel.ReportHub.Handlers.Managers.Helpers;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Managers.Customer;

[TestFixture]
public class CustomerManagerTests : BaseTestFixture
{
    private Mock<ICustomerRepository> _customerRepositoryMock;
    private Mock<ICountryDataFiller> _countryDataFillerMock;
    private CustomerManager _customerManager;

    [SetUp]
    public void Setup()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _countryDataFillerMock = new Mock<ICountryDataFiller>();
        _customerManager = new CustomerManager(_customerRepositoryMock.Object, _countryDataFillerMock.Object, Mapper);
    }

    [Test]
    public async Task CreateCustomerAsync_ValidRequest_ReturnsCustomerDTO()
    {
        // Arrange
        var dto = Fixture.Create<CreateCustomerDTO>();

        // Act
        var result = await _customerManager.CreateCustomerAsync(dto, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(dto.Name));
        Assert.That(result.Email, Is.EqualTo(dto.Email));
        Assert.That(result.ClientId, Is.EqualTo(dto.ClientId));
        Assert.That(result.CountryId, Is.EqualTo(dto.CountryId));

        _countryDataFillerMock.Verify(
            x => x.FillCountryDataAsync(It.IsAny<IList<Data.Models.Customer>>(), CancellationToken.None),
            Times.Once);

        _customerRepositoryMock.Verify(
            x => x.AddManyAsync(It.IsAny<IList<Data.Models.Customer>>(), CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task CreateCustomersAsync_ValidRequest_ReturnsCustomerDTOs()
    {
        // Arrange
        var dtos = Fixture.CreateMany<CreateCustomerDTO>(3).ToList();

        // Act
        var results = await _customerManager.CreateCustomersAsync(dtos, CancellationToken.None);

        // Assert
        Assert.That(results, Has.Count.EqualTo(dtos.Count));

        foreach (var result in results)
        {
            var original = dtos.Single(x => x.Email == result.Email);
            Assert.That(result.Name, Is.EqualTo(original.Name));
            Assert.That(result.Email, Is.EqualTo(original.Email));
            Assert.That(result.ClientId, Is.EqualTo(original.ClientId));
            Assert.That(result.CountryId, Is.EqualTo(original.CountryId));
        }

        _countryDataFillerMock.Verify(
            x => x.FillCountryDataAsync(It.IsAny<IList<Data.Models.Customer>>(), CancellationToken.None),
            Times.Once);

        _customerRepositoryMock.Verify(
            x => x.AddManyAsync(It.Is<IList<Data.Models.Customer>>(list =>
                list.All(c => c.Id != Guid.Empty)), CancellationToken.None),
            Times.Once);
    }
}
