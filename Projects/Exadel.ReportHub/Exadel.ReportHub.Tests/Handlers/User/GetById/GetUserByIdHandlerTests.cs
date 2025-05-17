using AutoFixture;
using ErrorOr;
using Exadel.ReportHub.Handlers.User.GetById;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.User;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.User.GetById;

[TestFixture]
public class GetUserByIdHandlerTests : BaseTestFixture
{
    private Mock<IUserRepository> _userRepositoryMock;
    private GetUserByIdHandler _handler;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new GetUserByIdHandler(_userRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task GetUserById_ValidRequest_ReturnsUserDTO()
    {
        // Arrange
        var user = Fixture.Create<Data.Models.User>();
        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(user.Id, CancellationToken.None))
            .ReturnsAsync(user);

        // Act
        var request = new GetUserByIdRequest(user.Id);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.InstanceOf<UserDTO>());
        Assert.That(result.Value.Id, Is.EqualTo(user.Id));
        Assert.That(result.Value.Email, Is.EqualTo(user.Email));
        Assert.That(result.Value.FullName, Is.EqualTo(user.FullName));

        _userRepositoryMock.Verify(
            repo => repo.GetByIdAsync(user.Id, CancellationToken.None),
            Times.Once);
    }

    [Test]
    public async Task GetUserById_UserNotFound_ReturnsErrorNotFoundError()
    {
        // Arrange
        var user = Fixture.Create<Data.Models.User>();

        // Act
        var request = new GetUserByIdRequest(user.Id);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.Errors, Has.Count.EqualTo(1), "Should contains the only error");
        Assert.That(result.FirstError.Type, Is.EqualTo(ErrorType.NotFound));

        _userRepositoryMock.Verify(
            repo => repo.GetByIdAsync(user.Id, CancellationToken.None),
            Times.Once);
    }
}
