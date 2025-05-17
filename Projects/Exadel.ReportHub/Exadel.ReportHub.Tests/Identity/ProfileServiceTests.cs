using System.Security.Claims;
using AutoFixture;
using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Exadel.ReportHub.Data.Enums;
using Exadel.ReportHub.Identity;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Identity;

[TestFixture]
public class ProfileServiceTests : BaseTestFixture
{
    private const string Caller = "test";

    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IUserAssignmentRepository> _userAssignmentRepositoryMock;
    private ProfileService _profileService;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userAssignmentRepositoryMock = new Mock<IUserAssignmentRepository>();
        _profileService = new ProfileService(_userRepositoryMock.Object, _userAssignmentRepositoryMock.Object);
    }

    [Test]
    public async Task GetProfileDataAsync_Valid_AddsExpectedClaims()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = Fixture.Build<Data.Models.User>()
            .With(x => x.Id, userId)
            .Create();

        var roles = new List<UserRole>
        {
            UserRole.Operator,
            UserRole.ClientAdmin,
            UserRole.Owner,
            UserRole.SuperAdmin
        };

        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(JwtClaimTypes.Subject, userId.ToString()));
        var principal = new ClaimsPrincipal(identity);

        var context = new ProfileDataRequestContext
        {
            Subject = principal,
            IssuedClaims = []
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, CancellationToken.None))
            .ReturnsAsync(user);

        _userAssignmentRepositoryMock
            .Setup(x => x.GetUserRolesAsync(userId, CancellationToken.None))
            .ReturnsAsync(roles);

        // Act
        await _profileService.GetProfileDataAsync(context);

        // Assert
        Assert.That(context.IssuedClaims.Any(c => c.Type == JwtClaimTypes.Name && c.Value == user.FullName));
        Assert.That(context.IssuedClaims.Any(c => c.Type == JwtClaimTypes.Email && c.Value == user.Email));
        Assert.That(context.IssuedClaims.Count(c => c.Type == JwtClaimTypes.Role), Is.EqualTo(roles.Count));

        foreach (var role in roles)
        {
            Assert.That(context.IssuedClaims.Any(c => c.Type == JwtClaimTypes.Role && c.Value == role.ToString()), Is.True);
        }

        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId, CancellationToken.None), Times.Once);
        _userAssignmentRepositoryMock.Verify(x => x.GetUserRolesAsync(userId, CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task IsActiveAsync_WhenUserIsActive_ChecksTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(JwtClaimTypes.Subject, userId.ToString()));
        var principal = new ClaimsPrincipal(identity);

        var context = new IsActiveContext(principal, new Client(), Caller);

        _userRepositoryMock
            .Setup(x => x.IsActiveAsync(userId, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        await _profileService.IsActiveAsync(context);

        // Assert
        Assert.That(context.IsActive, Is.True);
        _userRepositoryMock.Verify(x => x.IsActiveAsync(userId, CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task IsActiveAsync_WhenUserIsInactive_ChecksFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(JwtClaimTypes.Subject, userId.ToString()));
        var principal = new ClaimsPrincipal(identity);

        var context = new IsActiveContext(principal, new Client(), Caller);

        _userRepositoryMock
            .Setup(x => x.IsActiveAsync(userId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        await _profileService.IsActiveAsync(context);

        // Assert
        Assert.That(context.IsActive, Is.False);
        _userRepositoryMock.Verify(x => x.IsActiveAsync(userId, CancellationToken.None), Times.Once);
    }
}
