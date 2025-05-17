using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Validators;

[TestFixture]
public class ImportCustomerValidatorTests : BaseTestFixture
{
    private ImportCustomerValidator _validator;
    private Mock<ICustomerRepository> _customerRepositoryMock;
    private Mock<ICountryRepository> _countryRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _countryRepositoryMock = new Mock<ICountryRepository>();
        var updateCustomerDtoValidator = new InlineValidator<UpdateCustomerDTO>();
        updateCustomerDtoValidator.RuleSet("Default", () =>
        {
            updateCustomerDtoValidator.RuleLevelCascadeMode = CascadeMode.Stop;

            updateCustomerDtoValidator.RuleFor(x => x.CountryId)
                .NotEmpty()
                .MustAsync(async (countryId, cancellationToken) =>
                    await _countryRepositoryMock.Object.ExistsAsync(countryId, cancellationToken))
                .WithMessage(Constants.Validation.Country.DoesNotExist);
        });
        _validator = new ImportCustomerValidator(_customerRepositoryMock.Object, updateCustomerDtoValidator);
    }

    [Test]
    public async Task ValidateAsync_ValidCustomer_NoErrorReturned()
    {
        // Arrange
        var customer = SetupValidCustomer();

        // Act
        var result = await _validator.TestValidateAsync(customer);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Is.Empty);
    }

    [Test]
    [TestCase("")]
    [TestCase(null)]
    public async Task ValidateAsync_EmptyEmail_ErrorReturned(string email)
    {
        // Arrange
        var customer = SetupValidCustomer();
        customer.Email = email;

        // Act
        var result = await _validator.TestValidateAsync(customer);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(ImportCustomerDTO.Email)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo($"'{nameof(customer.Email)}' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_InvalidEmailFormat_ErrorReturned()
    {
        // Arrange
        var customer = SetupValidCustomer();
        customer.Email = "customer";

        // Act
        var result = await _validator.TestValidateAsync(customer);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(ImportCustomerDTO.Email)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Email.IsInvalid));
    }

    [Test]
    public async Task ValidateAsync_EmailIsTaken_ErrorReturned()
    {
        // Arrange
        var customer = SetupValidCustomer();

        _customerRepositoryMock.Setup(x => x.EmailExistsAsync(customer.Email, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var result = await _validator.TestValidateAsync(customer);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(ImportCustomerDTO.Email)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Email.IsTaken));
    }

    [Test]
    public async Task ValidateAsync_EmptyCountryId_ErrorReturned()
    {
        // Arrange
        var customer = SetupValidCustomer();
        customer.CountryId = Guid.Empty;

        // Act
        var result = await _validator.TestValidateAsync(customer);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(ImportCustomerDTO.CountryId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Country Id' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_CountryIdDoesNotExist_ErrorReturned()
    {
        // Arrange
        var customer = SetupValidCustomer();

        _countryRepositoryMock
            .Setup(x => x.ExistsAsync(customer.CountryId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.TestValidateAsync(customer);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(ImportCustomerDTO.CountryId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Country.DoesNotExist));
    }

    private ImportCustomerDTO SetupValidCustomer()
    {
        var email = "customer@test.com";
        var customer = Fixture.Build<ImportCustomerDTO>()
            .With(x => x.Email, email)
            .Create();

        _countryRepositoryMock
            .Setup(x => x.ExistsAsync(customer.CountryId, CancellationToken.None))
            .ReturnsAsync(true);

        _customerRepositoryMock
            .Setup(x => x.EmailExistsAsync(customer.Email, CancellationToken.None))
            .ReturnsAsync(false);

        return customer;
    }
}
