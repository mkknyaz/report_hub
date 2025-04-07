using AutoFixture;
using Exadel.ReportHub.Handlers.User.GetActive;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.User.GetActive;

[TestFixture]
public class GetActiveUsersHandlerTests : BaseTestFixture
{
    private Mock<IUserRepository> _userRepositoryMock;
    private GetActiveUsersHandler _handler;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new GetActiveUsersHandler(_userRepositoryMock.Object, Mapper);
    }

    [Test]
    public async Task GetActiveUsers_ValidRequest_ReturnsActiveUserDTOs()
    {
        // Arrange
        var users = Fixture.Build<Data.Models.User>().With(x => x.IsActive, true).CreateMany(30).ToList();
        _userRepositoryMock
            .Setup(repo => repo.GetAllActiveAsync(CancellationToken.None))
            .ReturnsAsync(users);

        // Act
        var request = new GetActiveUsersRequest();
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.ToList(), Has.Count.EqualTo(users.Count));

        _userRepositoryMock.Verify(
            mock => mock.GetAllActiveAsync(CancellationToken.None),
            Times.Once);
    }
}
