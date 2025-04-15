using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using Exadel.ReportHub.SDK.DTOs.User;
using FluentValidation;
using FluentValidation.TestHelper;

namespace Exadel.ReportHub.Tests.Validators;

[TestFixture]
public class PasswordValidatorTests
{
    private IValidator<string> _passwordValidator;

    [SetUp]
    public void Setup()
    {
        _passwordValidator = new PasswordValidator();
    }

    [Test]
    public async Task ValidateAsync_PasswordIsEmpty_ErrorReturned()
    {
        var result = await _passwordValidator.TestValidateAsync(string.Empty);
        result.ShouldHaveValidationErrorFor(nameof(CreateUserDTO.Password))
            .WithErrorMessage($"'{nameof(CreateUserDTO.Password)}' must not be empty.");
        Assert.That(result.Errors.Count, Is.EqualTo(1));
    }

    [Test]
    [TestCase("Test1!")]
    [TestCase("test1234!", Constants.Validation.User.PasswordUppercaseMessage)]
    [TestCase("TEST1234!", Constants.Validation.User.PasswordLowercaseMessage)]
    [TestCase("Testtest!", Constants.Validation.User.PasswordDigitMessage)]
    [TestCase("Test12345", Constants.Validation.User.PasswordSpecialCharacterMessage)]
    public async Task ValidateAsync_PasswordIsInvalid_ErrorReturned(string password, string expectedMessage = null)
    {
        if(expectedMessage == null)
        {
            expectedMessage = GetPasswordLengthMessage(password.Length);
        }

        var result = await _passwordValidator.TestValidateAsync(password);
        result.ShouldHaveValidationErrorFor(nameof(CreateUserDTO.Password))
            .WithErrorMessage(expectedMessage);
        Assert.That(result.Errors.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task ValidateAsync_PasswordIsValid_NoErrorReturned()
    {
        var password = "Test1234!";
        var result = await _passwordValidator.TestValidateAsync(password);
        result.ShouldNotHaveAnyValidationErrors();
        Assert.That(result.Errors, Is.Empty);
    }

    private static string GetPasswordLengthMessage(int totalLength) =>
    $"The length of '{nameof(CreateUserDTO.Password)}' must be at least 8 characters. You entered {totalLength} characters.";
}
