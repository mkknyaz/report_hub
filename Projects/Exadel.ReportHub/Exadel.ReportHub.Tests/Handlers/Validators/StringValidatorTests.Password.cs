using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using FluentValidation;
using FluentValidation.TestHelper;

namespace Exadel.ReportHub.Tests.Handlers.Validators;

public partial class StringValidatorTests
{
    [TestFixture]
    public class Password
    {
        private IValidator<string> _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new StringValidator();
        }

        [Test]
        public async Task ValidateAsync_PasswordIsEmpty_ErrorReturned()
        {
            // Arrange
            var password = string.Empty;

            // Act
            var result = await _validator.TestValidateAsync(password, options => options.IncludeRuleSets(Constants.Validation.RuleSet.Passwords));

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo($"'{nameof(Constants.Validation.Password)}' must not be empty."));
        }

        [TestCase("Test1!")]
        [TestCase("test1234!", Constants.Validation.Password.RequireUppercase)]
        [TestCase("TEST1234!", Constants.Validation.Password.RequireLowercase)]
        [TestCase("Testtest!", Constants.Validation.Password.RequireDigit)]
        [TestCase("Test12345", Constants.Validation.Password.RequireSpecialCharacter)]
        public async Task ValidateAsync_InvalidPassword_ShouldReturnExpectedError(string password, string expectedMessage = null)
        {
            // Arrange
            if (expectedMessage == null)
            {
                expectedMessage = GetPasswordLengthMessage(password.Length);
            }

            // Act
            var result = await _validator.TestValidateAsync(password, options =>
                options.IncludeRuleSets(Constants.Validation.RuleSet.Passwords));

            // Assert
            result.ShouldHaveValidationErrorFor(nameof(Constants.Validation.Password))
                .WithErrorMessage(expectedMessage);
            Assert.That(result.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task ValidateAsync_PasswordIsValid_NoErrorReturned()
        {
            // Arrange
            var password = "Test1234!";

            // Act
            var result = await _validator.TestValidateAsync(password, options => options.IncludeRuleSets(Constants.Validation.RuleSet.Passwords));

            // Assert
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Errors, Is.Empty);
        }

        private static string GetPasswordLengthMessage(int totalLength) =>
        $"The length of '{nameof(Constants.Validation.Password)}' must be at least 8 characters. You entered {totalLength} characters.";
    }
}
