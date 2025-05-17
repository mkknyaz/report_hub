using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using FluentValidation;
using FluentValidation.TestHelper;

namespace Exadel.ReportHub.Tests.Handlers.Validators;

public partial class StringValidatorTests
{
    [TestFixture]
    public class Name
    {
        private IValidator<string> _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new StringValidator();
        }

        [Test]
        public async Task ValidateAsync_ValidName_NoErrorReturned()
        {
            // Arrange
            var name = "John";

            // Act
            var result = await _validator.TestValidateAsync(name, options => options.IncludeRuleSets(Constants.Validation.RuleSet.Names));

            // Assert
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Errors, Is.Empty);
        }

        [Test]
        public async Task ValidateAsync_EmptyName_ErrorReturned()
        {
            // Arrange
            var name = string.Empty;

            // Act
            var result = await _validator.TestValidateAsync(name, options => options.IncludeRuleSets(Constants.Validation.RuleSet.Names));

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Name' must not be empty."));
        }

        [Test]
        public async Task ValidateAsync_NameTooLong_ErrorReturned()
        {
            // Arrange
            var name = new string('A', Constants.Validation.Name.MaxLength + 1);

            // Act
            var result = await _validator.TestValidateAsync(name, options => options.IncludeRuleSets(Constants.Validation.RuleSet.Names));

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo($"The length of 'Name' must be {Constants.Validation.Name.MaxLength} characters or fewer. You entered {name.Length} characters."));
        }

        [Test]
        public async Task ValidateAsync_NameDoesNotStartWithCapital_ErrorReturned()
        {
            // Arrange
            var name = "john";

            // Act
            var result = await _validator.TestValidateAsync(name, options => options.IncludeRuleSets(Constants.Validation.RuleSet.Names));

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Name.MustStartWithCapital));
        }
    }
}
