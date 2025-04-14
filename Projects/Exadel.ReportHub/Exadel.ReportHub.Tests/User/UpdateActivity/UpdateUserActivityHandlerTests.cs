using AutoFixture;
using ErrorOr;
using Exadel.ReportHub.Handlers.User.UpdateActivity;
using Exadel.ReportHub.RA.Abstract;
using Moq;

namespace Exadel.ReportHub.Tests.User.UpdateActivity;

[TestFixture]
public class UpdateUserActivityHandlerTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private UpdateUserActivityHandler _handler;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new UpdateUserActivityHandler(_userRepositoryMock.Object);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task UpdateUserActivity_WhenUserExists_ReturnsUpdated(bool isActive)
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock
            .Setup(x => x.ExistsAsync(userId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var request = new UpdateUserActivityRequest(userId, isActive);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.EqualTo(Result.Updated));

        _userRepositoryMock.Verify(
            x => x.UpdateActivityAsync(userId, isActive, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task UpdateUserActivity_WhenUserNotExists_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock
            .Setup(x => x.ExistsAsync(userId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var request = new UpdateUserActivityRequest(userId, true);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.Errors, Has.Count.EqualTo(1), "Should contains the only error");
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _userRepositoryMock.Verify(
            x => x.UpdateActivityAsync(It.IsAny<Guid>(), It.IsAny<bool>(), CancellationToken.None),
            Times.Never);
    }
}
