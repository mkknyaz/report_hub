using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Exadel.ReportHub.Common.Authorization;
using Exadel.ReportHub.Data.Enums;
using Exadel.ReportHub.Host.PolicyHandlers;
using Exadel.ReportHub.RA.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Exadel.ReportHub.Tests.Host.PolicyHandlers;

[TestFixture]
public class PermissionHandlerTests
{
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private Mock<IUserAssignmentRepository> _userAssignmentRepoMock;
    private Mock<ILogger<PermissionHandler>> _loggerMock;
    private DefaultHttpContext _httpContext;
    private PermissionHandler _handler;
    private ClaimsPrincipal _user;

    [SetUp]
    public void SetUp()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _userAssignmentRepoMock = new Mock<IUserAssignmentRepository>();
        _loggerMock = new Mock<ILogger<PermissionHandler>>();

        _httpContext = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContext);

        _handler = new PermissionHandler(
            _httpContextAccessorMock.Object,
            _userAssignmentRepoMock.Object,
            _loggerMock.Object);

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, UserRole.ClientAdmin.ToString())
        }, "TestAuth");

        _user = new ClaimsPrincipal(identity);
    }

    [Test]
    public async Task PermissionHandler_UnauthenticatedUser_DoesNotSucceed()
    {
        // Arrange
        var context = new AuthorizationHandlerContext(
            new[] { new PermissionRequirement(Permission.Read) },
            new ClaimsPrincipal(),
            null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.False);
    }

    [Test]
    public async Task PermissionHandler_NoControllerRoute_DoesNotSucceed()
    {
        // Arrange
        _httpContext.Request.RouteValues.Clear();

        var context = new AuthorizationHandlerContext(
            new[] { new PermissionRequirement(Permission.Read) },
            _user,
            null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.False);
    }

    [Test]
    public async Task PermissionHandler_NoMatchingRoles_DoesNotSucceed()
    {
        // Arrange
        _httpContext.Request.RouteValues["controller"] = "ClientsService";

        var userWithNoMatchingRoles = new ClaimsPrincipal(
            new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "TestAuth"));

        var context = new AuthorizationHandlerContext(
            new[] { new PermissionRequirement(Permission.Create) },
            userWithNoMatchingRoles,
            null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.False);
    }

    [Test]
    public async Task PermissionHandler_ValidClientIdInQuery_Succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        _httpContext.Request.RouteValues["controller"] = "ClientsService";
        _httpContext.Request.QueryString = new QueryString($"?clientId={clientId}");

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, UserRole.ClientAdmin.ToString())
            }, "TestAuth"));

        _userAssignmentRepoMock.Setup(x =>
            x.ExistAnyAsync(userId, It.Is<List<Guid>>(l => l.Contains(clientId)), It.IsAny<List<UserRole>>(), CancellationToken.None))
            .ReturnsAsync(true);

        var context = new AuthorizationHandlerContext(
            new[] { new PermissionRequirement(Permission.Read) },
            user,
            null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.True);
    }

    [Test]
    public async Task PermissionHandler_ValidClientIdInBody_Succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        var json = JsonSerializer.Serialize(new { clientId });
        var body = new MemoryStream(Encoding.UTF8.GetBytes(json));
        _httpContext.Request.Body = body;
        _httpContext.Request.ContentType = "application/json";
        _httpContext.Request.RouteValues["controller"] = "ClientsService";

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, UserRole.ClientAdmin.ToString())
            }, "TestAuth"));

        _userAssignmentRepoMock.Setup(x =>
            x.ExistAnyAsync(userId, It.Is<List<Guid>>(l => l.Contains(clientId)), It.IsAny<List<UserRole>>(), CancellationToken.None))
            .ReturnsAsync(true);

        var context = new AuthorizationHandlerContext(
            new[] { new PermissionRequirement(Permission.Read) },
            user,
            null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.That(context.HasSucceeded, Is.True);
    }
}
