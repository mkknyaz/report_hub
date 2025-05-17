using System.Security.Claims;
using Exadel.ReportHub.Common.Exceptions;
using Exadel.ReportHub.RA;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Exadel.ReportHub.Tests.Common.Providers;

[TestFixture]
public class UserProviderTests
{
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private UserProvider _userProvider;

    [SetUp]
    public void SetUp()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _userProvider = new UserProvider(_httpContextAccessorMock.Object);
    }

    [Test]
    public void GetUserId_ValidClaim_ReturnsUserId()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns(httpContext);

        // Act
        var result = _userProvider.GetUserId();

        // Assert
        Assert.That(result, Is.EqualTo(Guid.Parse(userId)));
    }

    [Test]
    public void GetUserId_MissingClaim_ThrowsHttpStatusCodeException()
    {
        // Arrange
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns(httpContext);

        // Act & Assert
        var exception = Assert.Throws<HttpStatusCodeException>(() => _userProvider.GetUserId());
        Assert.That(exception!.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
    }
}
