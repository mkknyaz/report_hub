using AutoFixture;
using Exadel.ReportHub.Handlers.Country.GetAll;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Country.GetAll;

[TestFixture]
public class GetAllCountriesHandlerTests : BaseTestFixture
{
    private GetAllCountriesHandler _handler;
    private Mock<ICountryRepository> _countryRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _countryRepositoryMock = new Mock<ICountryRepository>();
        _handler = new GetAllCountriesHandler(_countryRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task GetCountries_ValidRequest_ReturnsCountryDto()
    {
        // Arrange
        var countries = Fixture.CreateMany<Data.Models.Country>(2).ToList();
        _countryRepositoryMock
            .Setup(x => x.GetAllAsync(CancellationToken.None))
            .ReturnsAsync(countries);

        // Act
        var request = new GetAllCountriesRequest();
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.ToList(), Has.Count.EqualTo(countries.Count));

        for (int i = 0; i < result.Value.Count; i++)
        {
            Assert.Multiple(() =>
            {
                Assert.That(result.Value[i].Id, Is.EqualTo(countries[i].Id));
                Assert.That(result.Value[i].Name, Is.EqualTo(countries[i].Name));
                Assert.That(result.Value[i].CurrencyCode, Is.EqualTo(countries[i].CurrencyCode));
                Assert.That(result.Value[i].CurrencyId, Is.EqualTo(countries[i].CurrencyId));
            });
        }

        _countryRepositoryMock.Verify(
            x => x.GetAllAsync(CancellationToken.None),
            Times.Once);
    }
}
