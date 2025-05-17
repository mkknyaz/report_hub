using ErrorOr;
using Exadel.ReportHub.Handlers.Client.UpdateName;
using Exadel.ReportHub.RA.Abstract;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Client.UpdateName;

[TestFixture]
public class UpdateClientNameHandlerTests
{
    private UpdateClientNameHandler _handler;
    private Mock<IClientRepository> _clientRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _clientRepositoryMock = new Mock<IClientRepository>();
        _handler = new UpdateClientNameHandler(_clientRepositoryMock.Object);
    }

    [Test]
    public async Task UpdateClientName_ValidRequest_ReturnsUpdated()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var newName = "Organization Inc.";

        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(clientId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var request = new UpdateClientNameRequest(clientId, newName);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.EqualTo(Result.Updated));

        _clientRepositoryMock.Verify(
            x => x.ExistsAsync(clientId, CancellationToken.None),
            Times.Once);

        _clientRepositoryMock.Verify(
            x => x.UpdateNameAsync(clientId, newName, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task UpdateClientName_ClientDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var clientId = Guid.NewGuid();
        var newName = "Organization Inc.";

        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(clientId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var request = new UpdateClientNameRequest(clientId, newName);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.FirstError, Is.EqualTo(Error.NotFound()));

        _clientRepositoryMock.Verify(
            x => x.ExistsAsync(clientId, CancellationToken.None),
            Times.Once);

        _clientRepositoryMock.Verify(
            x => x.UpdateNameAsync(clientId, newName, CancellationToken.None),
            Times.Never);
    }
}
