using AutoFixture;
using ErrorOr;
using Exadel.ReportHub.Handlers.Client.GetById;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Client.GetById;

[TestFixture]
public class GetClientByIdHandlerTests : BaseTestFixture
{
    private GetClientByIdHandler _handler;
    private Mock<IClientRepository> _clientRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _clientRepositoryMock = new Mock<IClientRepository>();
        _handler = new GetClientByIdHandler(_clientRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task GetClientById_ValidRequest_ClientReturned()
    {
        // Arrange
        var client = Fixture.Create<Data.Models.Client>();

        _clientRepositoryMock
            .Setup(x => x.GetByIdAsync(client.Id, CancellationToken.None))
            .ReturnsAsync(client);

        // Act
        var request = new GetClientByIdRequest(client.Id);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Not.Null);

        Assert.That(result.Value.Id, Is.EqualTo(client.Id));
        Assert.That(result.Value.BankAccountNumber, Is.EqualTo(client.BankAccountNumber));
        Assert.That(result.Value.Name, Is.EqualTo(client.Name));
        Assert.That(result.Value.Country, Is.EqualTo(client.Country));
        Assert.That(result.Value.CountryId, Is.EqualTo(client.CountryId));
        Assert.That(result.Value.CurrencyId, Is.EqualTo(client.CurrencyId));
        Assert.That(result.Value.CurrencyCode, Is.EqualTo(client.CurrencyCode));

        _clientRepositoryMock.Verify(
            x => x.GetByIdAsync(client.Id, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task GetClientById_ClientNotFound_ReturnsNotFound()
    {
        // Arrange
        var client = Fixture.Create<Data.Models.Client>();

        // Act
        var request = new GetClientByIdRequest(client.Id);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.Errors, Has.Count.EqualTo(1), "Should contains the only error");
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _clientRepositoryMock.Verify(
            repo => repo.GetByIdAsync(client.Id, CancellationToken.None),
            Times.Once);
    }
}
