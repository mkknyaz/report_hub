using AutoFixture;
using Exadel.ReportHub.Handlers.Managers.Helpers;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Managers.Helpers;

[TestFixture]
public class CountryDataFillerTests : BaseTestFixture
{
    private Mock<ICountryRepository> _countryRepositoryMock;

    private CountryDataFiller _countryDataFiller;

    [SetUp]
    public void Setup()
    {
        _countryRepositoryMock = new Mock<ICountryRepository>();
        _countryDataFiller = new CountryDataFiller(_countryRepositoryMock.Object);
    }

    [Test]
    public async Task FillCountryDataAsync_ValidRequest_FillsCountryFieldsCorrectly()
    {
        // Arrange
        var countries = Fixture.CreateMany<Data.Models.Country>(3).ToList();

        var customers = countries.Select(country =>
        {
            var customer = Fixture.Build<Data.Models.Customer>()
                .With(c => c.CountryId, country.Id)
                .Create();

            return customer;
        }).ToList();

        _countryRepositoryMock
            .Setup(repo => repo.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), CancellationToken.None))
            .ReturnsAsync(countries);

        // Act
        await _countryDataFiller.FillCountryDataAsync(customers, CancellationToken.None);

        // Assert
        foreach (var customer in customers)
        {
            var expected = countries.Single(c => c.Id == customer.CountryId);
            Assert.That(customer.Country, Is.EqualTo(expected.Name));
            Assert.That(customer.CurrencyId, Is.EqualTo(expected.CurrencyId));
            Assert.That(customer.CurrencyCode, Is.EqualTo(expected.CurrencyCode));
        }

        _countryRepositoryMock.Verify(repo =>
            repo.GetByIdsAsync(It.Is<IEnumerable<Guid>>(ids =>
                ids.All(id => countries.Select(c => c.Id).Contains(id)) &&
                ids.Count() == countries.Count),
            CancellationToken.None), Times.Once);
    }
}
