using ErrorOr;
using Exadel.ReportHub.Common.Exceptions;
using Exadel.ReportHub.Common.Providers;
using Exadel.ReportHub.Handlers.User.UpdatePassword;
using Exadel.ReportHub.RA.Abstract;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.User.UpdatePassword;

[TestFixture]
public class UpdateUserPasswordHandlerTests
{
    private const string Password = "TestPassword123!";
    private const string PasswordHash = "o3Kf/k0tBaqDfyxzB+c+DkEFGED3qas9wi49QOI93lfEmyWouVxClXybhCp+BNBsBur7PBoDBb2YyqDtZA2GNA==";
    private const string PasswordSalt = "qeTorxRgLG5/bExroDXYxw==";
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IUserProvider> _userProviderMock;
    private UpdateUserPasswordHandler _handler;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userProviderMock = new Mock<IUserProvider>();
        _handler = new UpdateUserPasswordHandler(_userRepositoryMock.Object, _userProviderMock.Object);
    }

    [Test]
    public async Task UpdateUserPassword_AuthenticatedUser_ReturnsUpdated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        string passwordHash = string.Empty;
        string passwordSalt = string.Empty;

        _userProviderMock
        .Setup(x => x.GetUserId())
        .Returns(userId);

        _userRepositoryMock
        .Setup(x => x.UpdatePasswordAsync(userId, It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None))
        .Callback<Guid, string, string, CancellationToken>((guid, hash, salt, cancellationToken) =>
        {
            passwordHash = hash;
            passwordSalt = salt;
        });

        // Act
        var result = await _handler.Handle(new UpdateUserPasswordRequest(Password), CancellationToken.None);

        // Assert
        Assert.That(passwordHash, Is.Not.Null.And.Not.Empty);
        Assert.That(passwordSalt, Is.Not.Null.And.Not.Empty);
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.EqualTo(Result.Updated));
        Assert.That(passwordHash, Has.Length.EqualTo(PasswordHash.Length));
        Assert.That(passwordSalt, Has.Length.EqualTo(PasswordSalt.Length));

        _userProviderMock.Verify(x => x.GetUserId(), Times.Once);
        _userRepositoryMock.Verify(
        x => x.UpdatePasswordAsync(userId, passwordHash, passwordSalt, CancellationToken.None), Times.Once);
    }

    [Test]
    public void UpdateUserPassword_UnauthenticatedUser_ReturnsNotFound()
    {
        // Arrange
        _userProviderMock.Setup(x => x.GetUserId())
            .Throws(new HttpStatusCodeException(StatusCodes.Status401Unauthorized));

        // Act
        var result = Assert.ThrowsAsync<HttpStatusCodeException>(async () =>
            await _handler.Handle(new UpdateUserPasswordRequest(Password), CancellationToken.None));

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
        _userProviderMock.Verify(x => x.GetUserId(), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdatePasswordAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None), Times.Never);
    }
}
