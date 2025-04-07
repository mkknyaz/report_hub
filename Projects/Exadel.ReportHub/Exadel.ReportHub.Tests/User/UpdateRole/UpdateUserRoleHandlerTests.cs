using AutoFixture;
using ErrorOr;
using Exadel.ReportHub.Data.Enums;
using Exadel.ReportHub.Handlers.User.UpdateRole;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.User.UpdateRole;

[TestFixture]
public class UpdateUserRoleHandlerTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private UpdateUserRoleHandler _handler;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new UpdateUserRoleHandler(_userRepositoryMock.Object);
    }

    [TestCase(UserRole.Regular)]
    [TestCase(UserRole.Admin)]
    public async Task UpdateUserRole_WhenUserExists_ReturnsUpdated(UserRole role)
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock
            .Setup(x => x.ExistsAsync(userId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var request = new UpdateUserRoleRequest(userId, role);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.EqualTo(Result.Updated));

        _userRepositoryMock.Verify(
            x => x.UpdateRoleAsync(userId, role, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task UpdateUserRole_WhenUserNotExists_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock
            .Setup(x => x.ExistsAsync(userId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var request = new UpdateUserRoleRequest(userId, It.IsAny<UserRole>());
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.Errors, Has.Count.EqualTo(1), "Should contains the only error");
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _userRepositoryMock.Verify(
            x => x.UpdateRoleAsync(It.IsAny<Guid>(), It.IsAny<UserRole>(), CancellationToken.None),
            Times.Never);
    }
}
