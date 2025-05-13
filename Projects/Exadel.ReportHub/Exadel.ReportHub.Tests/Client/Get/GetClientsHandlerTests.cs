using AutoFixture;
using Exadel.ReportHub.Handlers.Client.Get;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Client.Get;

[TestFixture]
public class GetClientsHandlerTests : BaseTestFixture
{
    private GetClientsHandler _handler;
    private Mock<IClientRepository> _clientRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _clientRepositoryMock = new Mock<IClientRepository>();
        _handler = new GetClientsHandler(_clientRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task GetClients_ValidRequest_ClientsReturned()
    {
        // Arrange
        var clients = Fixture.Build<Data.Models.Client>().CreateMany(30).ToList();
        _clientRepositoryMock
            .Setup(repo => repo.GetAsync(CancellationToken.None))
            .ReturnsAsync(clients);

        // Act
        var request = new GetClientsRequest();
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.ToList(), Has.Count.EqualTo(clients.Count));
        for (int i = 0; i < result.Value.Count; i++)
        {
            Assert.Multiple(() =>
            {
                Assert.That(result.Value[i].Id, Is.EqualTo(clients[i].Id));
                Assert.That(result.Value[i].Name, Is.EqualTo(clients[i].Name));
                Assert.That(result.Value[i].BankAccountNumber, Is.EqualTo(clients[i].BankAccountNumber));
                Assert.That(result.Value[i].Country, Is.EqualTo(clients[i].Country));
                Assert.That(result.Value[i].CountryId, Is.EqualTo(clients[i].CountryId));
                Assert.That(result.Value[i].CurrencyId, Is.EqualTo(clients[i].CurrencyId));
                Assert.That(result.Value[i].CurrencyCode, Is.EqualTo(clients[i].CurrencyCode));
            });
        }

        _clientRepositoryMock.Verify(
            mock => mock.GetAsync(CancellationToken.None),
            Times.Once);
    }
}
