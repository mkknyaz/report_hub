using ErrorOr;
using Exadel.ReportHub.Common.Providers;
using Exadel.ReportHub.Handlers.User.UpdateName;
using Exadel.ReportHub.RA.Abstract;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.User.UpdateName;

[TestFixture]
public class UpdateUserNameHandlerTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private UpdateUserNameHandler _handler;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new UpdateUserNameHandler(_userRepositoryMock.Object);
    }

    [Test]
    public async Task UpdateUserName_ValidRequest_ReturnsUpdated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newName = "New Name";
        var request = new UpdateUserNameRequest(userId, newName);
        _userRepositoryMock
            .Setup(x => x.UpdateNameAsync(userId, newName, CancellationToken.None));
        _userRepositoryMock
            .Setup(x => x.ExistsAsync(userId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.EqualTo(Result.Updated));
        _userRepositoryMock.Verify(x => x.ExistsAsync(userId, CancellationToken.None), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateNameAsync(userId, newName, CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task UpdateUserName_UserDoesntExist_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newName = "New Name";
        var request = new UpdateUserNameRequest(userId, newName);
        _userRepositoryMock
            .Setup(x => x.ExistsAsync(userId, CancellationToken.None))
            .ReturnsAsync(false);
        _userRepositoryMock
            .Setup(x => x.UpdateNameAsync(userId, newName, CancellationToken.None));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.True);
        Assert.That(result.FirstError, Is.EqualTo(Error.NotFound()));
        _userRepositoryMock.Verify(x => x.ExistsAsync(userId, CancellationToken.None), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateNameAsync(userId, newName, CancellationToken.None), Times.Never);
    }
}
