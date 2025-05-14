using AutoFixture;
using Exadel.ReportHub.Handlers.Client.Create;
using Exadel.ReportHub.Handlers.Managers.Client;
using Exadel.ReportHub.SDK.DTOs.Client;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Client.Create;

[TestFixture]
public class CreateClientHandlerTests : BaseTestFixture
{
    private Mock<IClientManager> _clientManagerMock;
    private CreateClientHandler _handler;

    [SetUp]
    public void Setup()
    {
        _clientManagerMock = new Mock<IClientManager>();
        _handler = new CreateClientHandler(_clientManagerMock.Object);
    }

    [Test]
    public async Task CreateClient_ValidRequest_ReturnsClientDto()
    {
        // Arrange
        var createClientDto = Fixture.Create<CreateClientDTO>();
        var clientDto = Fixture.Create<ClientDTO>();

        _clientManagerMock
            .Setup(x => x.CreateClientAsync(createClientDto, CancellationToken.None))
            .ReturnsAsync(clientDto);

        // Act
        var request = new CreateClientRequest(createClientDto);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.InstanceOf<ClientDTO>(), "Returned object should be an instance of ClientDTO");
        Assert.That(result.Value.Id, Is.EqualTo(clientDto.Id));
        Assert.That(result.Value.Name, Is.EqualTo(clientDto.Name));
        Assert.That(result.Value.BankAccountNumber, Is.EqualTo(clientDto.BankAccountNumber));
        Assert.That(result.Value.CountryId, Is.EqualTo(clientDto.CountryId));
        Assert.That(result.Value.Country, Is.EqualTo(clientDto.Country));
        Assert.That(result.Value.CurrencyCode, Is.EqualTo(clientDto.CurrencyCode));
        Assert.That(result.Value.CurrencyId, Is.EqualTo(clientDto.CurrencyId));
    }
}
