using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using FluentValidation;
using FluentValidation.TestHelper;

namespace Exadel.ReportHub.Tests.Validators;

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
            var result = await _validator.TestValidateAsync(string.Empty, options => options.IncludeRuleSets(Constants.Validation.RuleSet.Passwords));
            result.ShouldHaveValidationErrorFor(nameof(Constants.Validation.Password))
                .WithErrorMessage($"'{nameof(Constants.Validation.Password)}' must not be empty.");
            Assert.That(result.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        [TestCase("Test1!")]
        [TestCase("test1234!", Constants.Validation.Password.UppercaseMessage)]
        [TestCase("TEST1234!", Constants.Validation.Password.LowercaseMessage)]
        [TestCase("Testtest!", Constants.Validation.Password.DigitMessage)]
        [TestCase("Test12345", Constants.Validation.Password.SpecialCharacterMessage)]
        public async Task ValidateAsync_PasswordIsInvalid_ErrorReturned(string password, string expectedMessage = null)
        {
            if (expectedMessage == null)
            {
                expectedMessage = GetPasswordLengthMessage(password.Length);
            }

            var result = await _validator.TestValidateAsync(password, options => options.IncludeRuleSets(Constants.Validation.RuleSet.Passwords));
            result.ShouldHaveValidationErrorFor(nameof(Constants.Validation.Password))
                .WithErrorMessage(expectedMessage);
            Assert.That(result.Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task ValidateAsync_PasswordIsValid_NoErrorReturned()
        {
            var password = "Test1234!";
            var result = await _validator.TestValidateAsync(password, options => options.IncludeRuleSets(Constants.Validation.RuleSet.Passwords));
            result.ShouldNotHaveAnyValidationErrors();
            Assert.That(result.Errors, Is.Empty);
        }

        private static string GetPasswordLengthMessage(int totalLength) =>
        $"The length of '{nameof(Constants.Validation.Password)}' must be at least 8 characters. You entered {totalLength} characters.";
    }
}
