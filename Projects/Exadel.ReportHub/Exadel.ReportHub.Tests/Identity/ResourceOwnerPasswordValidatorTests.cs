using AutoFixture;
using Duende.IdentityServer.Validation;
using Exadel.ReportHub.Common;
using Exadel.ReportHub.Identity;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.Tests.Abstracts;
using Moq;

namespace Exadel.ReportHub.Tests.Identity;

[TestFixture]
public class ResourceOwnerPasswordValidatorTests : BaseTestFixture
{
    private Mock<IUserRepository> _userRepositoryMock;
    private ResourceOwnerPasswordValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _validator = new ResourceOwnerPasswordValidator(_userRepositoryMock.Object);
    }

    [Test]
    public async Task ValidateAsync_UserDoesNotExist_ShouldSetInvalidGrantResult()
    {
        // Arrange
        var context = new ResourceOwnerPasswordValidationContext
        {
            UserName = "test@test.com",
            Password = "password"
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(context.UserName, CancellationToken.None))
            .ReturnsAsync((Data.Models.User)null);

        // Act
        await _validator.ValidateAsync(context);

        // Assert
        Assert.That(context.Result.IsError, Is.True);
        Assert.That(context.Result.Error, Is.EqualTo("invalid_grant"));
        Assert.That(context.Result.ErrorDescription, Is.EqualTo("User does not exist."));
    }

    [Test]
    public async Task ValidateAsync_IncorrectPassword_ShouldSetInvalidGrantResult()
    {
        // Arrange
        var user = Fixture.Build<Data.Models.User>()
            .With(x => x.PasswordSalt, "salt")
            .Create();

        var context = new ResourceOwnerPasswordValidationContext
        {
            UserName = user.Email,
            Password = "wrongPassword"
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(user.Email, CancellationToken.None))
            .ReturnsAsync(user);

        // Act
        await _validator.ValidateAsync(context);

        // Assert
        Assert.That(context.Result.IsError, Is.True);
        Assert.That(context.Result.Error, Is.EqualTo("invalid_grant"));
        Assert.That(context.Result.ErrorDescription, Is.EqualTo("Incorrect password."));
    }

    [Test]
    public async Task ValidateAsync_ValidUserAndPassword_ShouldSetSuccessResult()
    {
        // Arrange
        var password = "validPassword";
        var salt = "salt";

        var user = Fixture.Build<Data.Models.User>()
            .With(u => u.PasswordSalt, salt)
            .With(u => u.PasswordHash, PasswordHasher.GetPasswordHash(password, salt))
            .Create();

        var context = new ResourceOwnerPasswordValidationContext
        {
            UserName = user.Email,
            Password = password
        };

        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(user.Email, CancellationToken.None))
            .ReturnsAsync(user);

        // Act
        await _validator.ValidateAsync(context);

        // Assert
        Assert.That(context.Result.IsError, Is.False);
        Assert.That(context.Result.Subject?.FindFirst("sub")?.Value, Is.EqualTo(user.Id.ToString()));
    }
}
