using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using FluentValidation;
using FluentValidation.TestHelper;

namespace Exadel.ReportHub.Tests.Handlers.Validators;

public partial class StringValidatorTests
{
    [TestFixture]
    public class Country
    {
        private IValidator<string> _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new StringValidator();
        }

        [Test]
        public async Task ValidateAsync_ValidCountry_NoErrorReturned()
        {
            // Arrange
            var country = "Georgia";

            // Act
            var result = await _validator.TestValidateAsync(country, options => options.IncludeRuleSets(Constants.Validation.RuleSet.Countries));

            // Assert
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.Errors, Is.Empty);
        }

        [Test]
        public async Task ValidateAsync_EmptyCountry_ErrorReturned()
        {
            // Arrange
            var country = string.Empty;

            // Act
            var result = await _validator.TestValidateAsync(country, options => options.IncludeRuleSets(Constants.Validation.RuleSet.Countries));

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Country' must not be empty."));
        }

        [Test]
        public async Task ValidateAsync_CountryTooLong_ErrorReturned()
        {
            // Arrange
            var country = new string('A', Constants.Validation.Country.MaxLength + 1);

            // Act
            var result = await _validator.TestValidateAsync(country, options => options.IncludeRuleSets(Constants.Validation.RuleSet.Countries));

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors[0].ErrorMessage,
                Is.EqualTo($"The length of 'Country' must be {Constants.Validation.Country.MaxLength} characters or fewer. You entered {country.Length} characters."));
        }

        [Test]
        public async Task ValidateAsync_CountryDoesNotStartWithCapital_ErrorReturned()
        {
            // Arrange
            var country = "georgia";

            // Act
            var result = await _validator.TestValidateAsync(country, options => options.IncludeRuleSets(Constants.Validation.RuleSet.Countries));

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Country.MustStartWithCapital));
        }
    }
}
