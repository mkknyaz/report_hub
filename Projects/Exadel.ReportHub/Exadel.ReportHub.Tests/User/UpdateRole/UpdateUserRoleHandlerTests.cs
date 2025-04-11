using ErrorOr;
using Exadel.ReportHub.Data.Enums;
using Exadel.ReportHub.Handlers.User.UpdateRole;
using Exadel.ReportHub.RA.Abstract;
using Moq;

namespace Exadel.ReportHub.Tests.User.UpdateRole;

[TestFixture]
public class UpdateUserRoleHandlerTests
{
    private Mock<IUserAssignmentRepository> _userAssignmentRepositoryMock;
    private UpdateUserRoleHandler _handler;

    [SetUp]
    public void Setup()
    {
        _userAssignmentRepositoryMock = new Mock<IUserAssignmentRepository>();
        _handler = new UpdateUserRoleHandler(_userAssignmentRepositoryMock.Object);
    }

    [TestCase(UserRole.Regular)]
    [TestCase(UserRole.ClientAdmin)]
    [TestCase(UserRole.SuperAdmin)]
    public async Task UpdateUserRole_WhenUserExists_ReturnsUpdated(UserRole role)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        _userAssignmentRepositoryMock
            .Setup(x => x.ExistsAsync(userId, clientId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var request = new UpdateUserRoleRequest(userId, clientId, role);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.EqualTo(Result.Updated));

        _userAssignmentRepositoryMock.Verify(
            x => x.UpdateRoleAsync(userId, clientId, role, CancellationToken.None),
            Times.Once);
    }

    [TestCase(UserRole.Regular)]
    [TestCase(UserRole.ClientAdmin)]
    [TestCase(UserRole.SuperAdmin)]
    public async Task UpdateUserRole_WhenUserNotExists_ReturnsNotFound(UserRole role)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        _userAssignmentRepositoryMock
            .Setup(x => x.ExistsAsync(userId, clientId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var request = new UpdateUserRoleRequest(userId, clientId, role);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.Errors, Has.Count.EqualTo(1), "Should contains the only error");
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _userAssignmentRepositoryMock.Verify(
            x => x.UpdateRoleAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UserRole>(), CancellationToken.None),
            Times.Never);
    }
}
