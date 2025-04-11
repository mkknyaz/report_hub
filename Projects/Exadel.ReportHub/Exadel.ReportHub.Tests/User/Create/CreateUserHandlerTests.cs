using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.User.Create;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.User;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.User.Create;

[TestFixture]
public class CreateUserHandlerTests : BaseTestFixture
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IUserAssignmentRepository> _userAssignmentRepositoryMock;
    private CreateUserHandler _handler;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userAssignmentRepositoryMock = new Mock<IUserAssignmentRepository>();
        _handler = new CreateUserHandler(_userRepositoryMock.Object, _userAssignmentRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task CreateUser_ValidRequest_ReturnsUserDTO()
    {
        // Arrange
        var createUserDto = Fixture.Create<CreateUserDTO>();

        // Act
        var createUserRequest = new CreateUserRequest(createUserDto);
        var result = await _handler.Handle(createUserRequest, CancellationToken.None);

        // Assert
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.InstanceOf<UserDTO>(), "Returned object should be an instance of UserDTO");
        Assert.That(result.Value.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(result.Value.Email, Is.EqualTo(createUserDto.Email));
        Assert.That(result.Value.FullName, Is.EqualTo(createUserDto.FullName));

        _userRepositoryMock.Verify(
            mock => mock.AddAsync(
                It.Is<Data.Models.User>(
                    u => u.Email == createUserDto.Email &&
                    u.FullName == createUserDto.FullName &&
                    u.PasswordHash != string.Empty &&
                    u.PasswordSalt != string.Empty),
                CancellationToken.None),
            Times.Once);

        _userAssignmentRepositoryMock.Verify(
            mock => mock.AddAsync(
                It.Is<Data.Models.UserAssignment>(
                    ua => ua.UserId == result.Value.Id &&
                    ua.ClientId == Constants.Client.GlobalId &&
                    ua.Role == Data.Enums.UserRole.Regular),
                CancellationToken.None),
            Times.Once);
    }
}
