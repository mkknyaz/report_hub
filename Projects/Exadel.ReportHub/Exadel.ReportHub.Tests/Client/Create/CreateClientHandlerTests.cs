using AutoFixture;
using Exadel.ReportHub.Data.Models;
using Exadel.ReportHub.Handlers.Client.Create;
using Exadel.ReportHub.Handlers.Managers.Common;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Client;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Client.Create;

[TestFixture]
public class CreateClientHandlerTests : BaseTestFixture
{
    private Mock<IClientRepository> _clientRepositoryMock;
    private Mock<ICountryBasedEntityManager> _countryBasedEntityManagerMock;
    private CreateClientHandler _handler;

    [SetUp]
    public void Setup()
    {
        _clientRepositoryMock = new Mock<IClientRepository>();
        _countryBasedEntityManagerMock = new Mock<ICountryBasedEntityManager>();
        _handler = new CreateClientHandler(_clientRepositoryMock.Object, Mapper, _countryBasedEntityManagerMock.Object);
    }

    [Test]
    public async Task CreateClient_ValidRequest_ReturnsClientDto()
    {
        // Arrange
        var createClientDto = Fixture.Create<CreateClientDTO>();
        var country = Fixture.Build<Country>().With(x => x.Id, createClientDto.CountryId).Create();

        var client = Fixture.Build<Data.Models.Client>()
                .With(x => x.Id, Guid.NewGuid())
                .With(x => x.Name, createClientDto.Name)
                .With(x => x.BankAccountNumber, createClientDto.BankAccountNumber)
                .With(x => x.CountryId, createClientDto.CountryId)
                .With(x => x.Country, country.Name)
                .With(x => x.CurrencyId, country.CurrencyId)
                .With(x => x.CurrencyCode, country.CurrencyCode)
                .Create();

        _countryBasedEntityManagerMock
            .Setup(x => x.GenerateEntityAsync<CreateClientDTO, Data.Models.Client>(createClientDto, CancellationToken.None))
            .ReturnsAsync(client);

        // Act
        var request = new CreateClientRequest(createClientDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.InstanceOf<ClientDTO>(), "Returned object should be an instance of ClientDTO");
        Assert.That(result.Value.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(result.Value.Name, Is.EqualTo(createClientDto.Name));
        Assert.That(result.Value.BankAccountNumber, Is.EqualTo(createClientDto.BankAccountNumber));
        Assert.That(result.Value.CountryId, Is.EqualTo(createClientDto.CountryId));
        Assert.That(result.Value.Country, Is.EqualTo(client.Country));
        Assert.That(result.Value.CurrencyCode, Is.EqualTo(client.CurrencyCode));
        Assert.That(result.Value.CurrencyId, Is.EqualTo(client.CurrencyId));
        Assert.That(createClientDto.CountryId, Is.EqualTo(country.Id));

        _clientRepositoryMock.Verify(
            mock => mock.AddAsync(
                It.Is<Data.Models.Client>(
                    c => c.Name == createClientDto.Name &&
                    c.BankAccountNumber == createClientDto.BankAccountNumber &&
                    c.CountryId == createClientDto.CountryId &&
                    c.Country == client.Country &&
                    c.CurrencyCode == client.CurrencyCode &&
                    c.CurrencyId == client.CurrencyId),
                CancellationToken.None),
            Times.Once);

        _countryBasedEntityManagerMock.Verify(
            x => x.GenerateEntityAsync<CreateClientDTO, Data.Models.Client>(createClientDto, CancellationToken.None),
            Times.Once);
    }
}
