using AutoFixture;
using Exadel.ReportHub.Handlers.Managers.Client;
using Exadel.ReportHub.Handlers.Managers.Helpers;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Client;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Managers.Client;

[TestFixture]
public class ClientManagerTests : BaseTestFixture
{
    private Mock<IClientRepository> _clientRepositoryMock;
    private Mock<ICountryDataFiller> _countryDataFillerMock;
    private ClientManager _clientManager;

    [SetUp]
    public void Setup()
    {
        _clientRepositoryMock = new Mock<IClientRepository>();
        _countryDataFillerMock = new Mock<ICountryDataFiller>();
        _clientManager = new ClientManager(_clientRepositoryMock.Object, _countryDataFillerMock.Object, Mapper);
    }

    [Test]
    public async Task CreateClientAsync_ValidRequest_ReturnsClientDto()
    {
        // Arrange
        var dto = Fixture.Create<CreateClientDTO>();

        // Act
        var result = await _clientManager.CreateClientAsync(dto, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(dto.Name));
        Assert.That(result.BankAccountNumber, Is.EqualTo(dto.BankAccountNumber));
        Assert.That(result.CountryId, Is.EqualTo(dto.CountryId));

        _countryDataFillerMock.Verify(
            x => x.FillCountryDataAsync(It.Is<IList<Data.Models.Client>>(c => c.Count == 1), CancellationToken.None),
            Times.Once);

        _clientRepositoryMock.Verify(
            x => x.AddManyAsync(It.Is<IList<Data.Models.Client>>(c => c.Count == 1), CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task CreateClientsAsync_ValidRequest_ReturnsClientDtos()
    {
        // Arrange
        var dtos = Fixture.CreateMany<CreateClientDTO>(3).ToList();

        // Act
        var results = await _clientManager.CreateClientsAsync(dtos, CancellationToken.None);

        // Assert
        Assert.That(results, Has.Count.EqualTo(dtos.Count));

        foreach (var result in results)
        {
            var match = dtos.First(dto => dto.Name == result.Name && dto.BankAccountNumber == result.BankAccountNumber);
            Assert.That(result.CountryId, Is.EqualTo(match.CountryId));
        }

        _countryDataFillerMock.Verify(
            x => x.FillCountryDataAsync(It.Is<IList<Data.Models.Client>>(c => c.Count == dtos.Count), CancellationToken.None),
            Times.Once);

        _clientRepositoryMock.Verify(
            x => x.AddManyAsync(It.Is<IList<Data.Models.Client>>(c => c.Count == dtos.Count), CancellationToken.None),
            Times.Once);
    }
}
