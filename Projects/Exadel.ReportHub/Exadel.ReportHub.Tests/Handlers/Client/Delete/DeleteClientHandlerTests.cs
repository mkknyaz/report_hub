using ErrorOr;
using Exadel.ReportHub.Handlers.Client.Delete;
using Exadel.ReportHub.RA.Abstract;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Client.Delete;

[TestFixture]
public class DeleteClientHandlerTests
{
    private Mock<IClientRepository> _clientRepositoryMock;
    private DeleteClientHandler _handler;

    [SetUp]
    public void Setup()
    {
        _clientRepositoryMock = new Mock<IClientRepository>();
        _handler = new DeleteClientHandler(_clientRepositoryMock.Object);
    }

    [Test]
    public async Task DeleteClient_ClientExists_ReturnsDeleted()
    {
        // Arrange
        var clientId = Guid.NewGuid();

        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(clientId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var request = new DeleteClientRequest(clientId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.EqualTo(Result.Deleted));

        _clientRepositoryMock.Verify(
            x => x.ExistsAsync(clientId, CancellationToken.None),
            Times.Once);

        _clientRepositoryMock.Verify(
            x => x.SoftDeleteAsync(clientId, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task DeleteClient_ClientDoesNotExist_ReturnsNotFound()
    {// Arrange
        var clientId = Guid.NewGuid();

        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(clientId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var request = new DeleteClientRequest(clientId);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _clientRepositoryMock.Verify(
            x => x.ExistsAsync(clientId, CancellationToken.None),
            Times.Once);

        _clientRepositoryMock.Verify(
            x => x.SoftDeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
