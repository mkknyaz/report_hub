using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.UserAssignment.Upsert;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.UserAssignment;
using Exadel.ReportHub.SDK.Enums;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation.TestHelper;
using Moq;

namespace Exadel.ReportHub.Tests.UserAssignment.Upsert;

[TestFixture]
public class UpsertUserAssignmentValidatorTests : BaseTestFixture
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IClientRepository> _clientRepositoryMock;
    private UpsertUserAssignmentRequestValidator _validator;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _validator = new UpsertUserAssignmentRequestValidator(_userRepositoryMock.Object, _clientRepositoryMock.Object);
    }

    [TestCase(UserRole.Operator)]
    [TestCase(UserRole.ClientAdmin)]
    [TestCase(UserRole.Owner)]
    [TestCase(UserRole.SuperAdmin)]
    public async Task ValidateAsync_ValidRequest_NoErrorReturned(UserRole role)
    {
        // Arrange
        var clientId = role == UserRole.SuperAdmin ? Constants.ClientData.GlobalId : Guid.NewGuid();
        var upsertUserAssignmentDto = Fixture.Build<UpsertUserAssignmentDTO>()
            .With(x => x.Role, role).With(x => x.ClientId, clientId).Create();
        _userRepositoryMock
            .Setup(x => x.ExistsAsync(upsertUserAssignmentDto.UserId, CancellationToken.None))
            .ReturnsAsync(true);
        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(upsertUserAssignmentDto.ClientId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var upsertUserAssignmentRequest = new UpsertUserAssignmentRequest(upsertUserAssignmentDto);
        var result = await _validator.TestValidateAsync(upsertUserAssignmentRequest);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
        Assert.That(result.Errors, Is.Empty);

        _userRepositoryMock.Verify(
            x => x.ExistsAsync(upsertUserAssignmentDto.UserId, CancellationToken.None),
            Times.Once);
        _clientRepositoryMock.Verify(
            x => x.ExistsAsync(upsertUserAssignmentDto.ClientId, CancellationToken.None),
            Times.Once);
    }

    [TestCase(UserRole.Operator)]
    [TestCase(UserRole.ClientAdmin)]
    [TestCase(UserRole.Owner)]
    [TestCase(UserRole.SuperAdmin)]
    public async Task ValidateAsync_UserIdIsEmpty_ErrorReturned(UserRole role)
    {
        // Arrange
        var clientId = role == UserRole.SuperAdmin ? Constants.ClientData.GlobalId : Guid.NewGuid();
        var upsertUserAssignmentDto = Fixture.Build<UpsertUserAssignmentDTO>()
            .With(x => x.UserId, Guid.Empty).With(x => x.Role, role)
            .With(x => x.ClientId, clientId).Create();
        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(upsertUserAssignmentDto.ClientId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var upsertUserAssignmentRequest = new UpsertUserAssignmentRequest(upsertUserAssignmentDto);
        var result = await _validator.TestValidateAsync(upsertUserAssignmentRequest);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpsertUserAssignmentDto.UserId);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'User Id' must not be empty."));

        _userRepositoryMock.Verify(
            x => x.ExistsAsync(It.IsAny<Guid>(), CancellationToken.None),
            Times.Never);
        _clientRepositoryMock.Verify(
            x => x.ExistsAsync(It.IsAny<Guid>(), CancellationToken.None),
            Times.Never);
    }

    [TestCase(UserRole.Operator)]
    [TestCase(UserRole.ClientAdmin)]
    [TestCase(UserRole.Owner)]
    [TestCase(UserRole.SuperAdmin)]
    public async Task ValidateAsync_UserNotExist_ErrorReturned(UserRole role)
    {
        // Arrange
        var clientId = role == UserRole.SuperAdmin ? Constants.ClientData.GlobalId : Guid.NewGuid();
        var upsertUserAssignmentDto = Fixture.Build<UpsertUserAssignmentDTO>()
            .With(x => x.Role, role).With(x => x.ClientId, clientId).Create();
        _userRepositoryMock
            .Setup(x => x.ExistsAsync(upsertUserAssignmentDto.UserId, CancellationToken.None))
            .ReturnsAsync(false);
        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(upsertUserAssignmentDto.ClientId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var upsertUserAssignmentRequest = new UpsertUserAssignmentRequest(upsertUserAssignmentDto);
        var result = await _validator.TestValidateAsync(upsertUserAssignmentRequest);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpsertUserAssignmentDto.UserId);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.User.DoesNotExist));

        _userRepositoryMock.Verify(
            x => x.ExistsAsync(upsertUserAssignmentDto.UserId, CancellationToken.None),
            Times.Once);
        _clientRepositoryMock.Verify(
            x => x.ExistsAsync(It.IsAny<Guid>(), CancellationToken.None),
            Times.Never);
    }

    [TestCase(UserRole.Operator)]
    [TestCase(UserRole.ClientAdmin)]
    [TestCase(UserRole.Owner)]
    [TestCase(UserRole.SuperAdmin)]
    public async Task ValidateAsync_ClientIdIsEmpty_ErrorReturned(UserRole role)
    {
        // Arrange
        var upsertUserAssignmentDto = Fixture.Build<UpsertUserAssignmentDTO>()
            .With(x => x.ClientId, Guid.Empty).With(x => x.Role, role).Create();
        _userRepositoryMock
            .Setup(x => x.ExistsAsync(upsertUserAssignmentDto.UserId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var upsertUserAssignmentRequest = new UpsertUserAssignmentRequest(upsertUserAssignmentDto);
        var result = await _validator.TestValidateAsync(upsertUserAssignmentRequest);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpsertUserAssignmentDto.ClientId);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Client Id' must not be empty."));

        _userRepositoryMock.Verify(
            x => x.ExistsAsync(upsertUserAssignmentDto.UserId, CancellationToken.None),
            Times.Once);
        _clientRepositoryMock.Verify(
            x => x.ExistsAsync(It.IsAny<Guid>(), CancellationToken.None),
            Times.Never);
    }

    [TestCase(UserRole.Operator)]
    [TestCase(UserRole.ClientAdmin)]
    [TestCase(UserRole.Owner)]
    [TestCase(UserRole.SuperAdmin)]
    public async Task ValidateAsync_ClientNotExist_ErrorReturned(UserRole role)
    {
        // Arrange
        var clientId = role == UserRole.SuperAdmin ? Constants.ClientData.GlobalId : Guid.NewGuid();
        var upsertUserAssignmentDto = Fixture.Build<UpsertUserAssignmentDTO>()
            .With(x => x.Role, role).With(x => x.ClientId, clientId).Create();
        _userRepositoryMock
            .Setup(x => x.ExistsAsync(upsertUserAssignmentDto.UserId, CancellationToken.None))
            .ReturnsAsync(true);
        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(upsertUserAssignmentDto.ClientId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var upsertUserAssignmentRequest = new UpsertUserAssignmentRequest(upsertUserAssignmentDto);
        var result = await _validator.TestValidateAsync(upsertUserAssignmentRequest);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpsertUserAssignmentDto.ClientId);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Client.DoesNotExist));

        _userRepositoryMock.Verify(
            x => x.ExistsAsync(upsertUserAssignmentDto.UserId, CancellationToken.None),
            Times.Once);
        _clientRepositoryMock.Verify(
            x => x.ExistsAsync(upsertUserAssignmentDto.ClientId, CancellationToken.None),
            Times.Once);
    }

    [TestCase((UserRole)999)]
    [TestCase((UserRole)(-1))]
    public async Task ValidateAsync_InvalidUserRole_ErrorReturned(UserRole role)
    {
        // Arrange
        var upsertUserAssignmentDto = Fixture.Build<UpsertUserAssignmentDTO>().With(x => x.Role, role).Create();
        _userRepositoryMock
            .Setup(x => x.ExistsAsync(upsertUserAssignmentDto.UserId, CancellationToken.None))
            .ReturnsAsync(true);
        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(upsertUserAssignmentDto.ClientId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var upsertUserAssignmentRequest = new UpsertUserAssignmentRequest(upsertUserAssignmentDto);
        var result = await _validator.TestValidateAsync(upsertUserAssignmentRequest);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpsertUserAssignmentDto.Role);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo($"'Role' has a range of values which does not include '{role}'."));

        _userRepositoryMock.Verify(
            x => x.ExistsAsync(upsertUserAssignmentDto.UserId, CancellationToken.None),
            Times.Once);
        _clientRepositoryMock.Verify(
            x => x.ExistsAsync(upsertUserAssignmentDto.ClientId, CancellationToken.None),
            Times.Once);
    }

    [TestCase(UserRole.Operator)]
    [TestCase(UserRole.ClientAdmin)]
    [TestCase(UserRole.Owner)]
    public async Task ValidateAsync_ClientRoleAndGlobalId_ErrorReturned(UserRole role)
    {
        // Arrange
        var upsertUserAssignmentDto = Fixture.Build<UpsertUserAssignmentDTO>()
            .With(x => x.Role, role).With(x => x.ClientId, Constants.ClientData.GlobalId).Create();
        _userRepositoryMock
            .Setup(x => x.ExistsAsync(upsertUserAssignmentDto.UserId, CancellationToken.None))
            .ReturnsAsync(true);
        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(upsertUserAssignmentDto.ClientId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var upsertUserAssignmentRequest = new UpsertUserAssignmentRequest(upsertUserAssignmentDto);
        var result = await _validator.TestValidateAsync(upsertUserAssignmentRequest);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpsertUserAssignmentDto.ClientId);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.UserAssignment.ClientRoleAssignment));

        _userRepositoryMock.Verify(
            x => x.ExistsAsync(upsertUserAssignmentDto.UserId, CancellationToken.None),
            Times.Once);
        _clientRepositoryMock.Verify(
            x => x.ExistsAsync(upsertUserAssignmentDto.ClientId, CancellationToken.None),
            Times.Once);
    }

    [TestCase(UserRole.SuperAdmin)]
    public async Task ValidateAsync_GlobalRoleAndNonGlobalId_ErrorReturned(UserRole role)
    {
        // Arrange
        var upsertUserAssignmentDto = Fixture.Build<UpsertUserAssignmentDTO>().With(x => x.Role, role).Create();
        _userRepositoryMock
            .Setup(x => x.ExistsAsync(upsertUserAssignmentDto.UserId, CancellationToken.None))
            .ReturnsAsync(true);
        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(upsertUserAssignmentDto.ClientId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var upsertUserAssignmentRequest = new UpsertUserAssignmentRequest(upsertUserAssignmentDto);
        var result = await _validator.TestValidateAsync(upsertUserAssignmentRequest);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpsertUserAssignmentDto.ClientId);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.UserAssignment.GlobalRoleAssignment));

        _userRepositoryMock.Verify(
            x => x.ExistsAsync(upsertUserAssignmentDto.UserId, CancellationToken.None),
            Times.Once);
        _clientRepositoryMock.Verify(
            x => x.ExistsAsync(upsertUserAssignmentDto.ClientId, CancellationToken.None),
            Times.Once);
    }
}
