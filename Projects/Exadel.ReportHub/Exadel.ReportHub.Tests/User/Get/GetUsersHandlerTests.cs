using AutoFixture;
using Exadel.ReportHub.Handlers.User.Get;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.User.Get;

[TestFixture]
public class GetActiveUsersHandlerTests : BaseTestFixture
{
    private Mock<IUserRepository> _userRepositoryMock;
    private GetUsersHandler _handler;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new GetUsersHandler(_userRepositoryMock.Object, Mapper);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    [TestCase(null)]
    public async Task GetActiveUsers_ValidRequest_ReturnsActiveUserDTOs(bool? isActive)
    {
        // Arrange
        if(isActive == null)
        {
            Random random = new Random();
            isActive = random.Next(0, 2) == 1;
        }

        var users = Fixture.Build<Data.Models.User>().With(x => x.IsActive, isActive).CreateMany(30).ToList();
        _userRepositoryMock
            .Setup(repo => repo.GetAsync(isActive, CancellationToken.None))
            .ReturnsAsync(users);

        // Act
        var request = new GetUsersRequest(isActive);
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsError, Is.False);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.ToList(), Has.Count.EqualTo(users.Count));

        _userRepositoryMock.Verify(
            mock => mock.GetAsync(isActive, CancellationToken.None),
            Times.Once);
    }
}
