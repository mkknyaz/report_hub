using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;

namespace Exadel.ReportHub.Tests.Validators;

public class UpdateCustomerValidatorTests : BaseTestFixture
{
    private UpdateCustomerValidator _validator;
    private Mock<ICountryRepository> _countryRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _countryRepositoryMock = new Mock<ICountryRepository>();
        var stringValidator = new InlineValidator<string>();
        stringValidator.RuleSet(Constants.Validation.RuleSet.Names, () =>
        {
            stringValidator.RuleFor(x => x)
                .NotEmpty()
                .MaximumLength(Constants.Validation.Name.MaxLength)
                .Matches("^[A-Z]")
                .WithMessage(Constants.Validation.Name.MustStartWithCapital)
                .WithName(nameof(Constants.Validation.Name));
        });
        _validator = new UpdateCustomerValidator(_countryRepositoryMock.Object, stringValidator);
    }

    [Test]
    public async Task ValidateAsync_EmptyCountryId_ErrorReturned()
    {
        // Arrange
        var customer = GetValidCustomer();
        customer.CountryId = Guid.Empty;

        // Act
        var result = await _validator.TestValidateAsync(customer);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateCustomerDTO.CountryId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Country Id' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_CountryIdDoesNotExist_ErrorReturned()
    {
        // Arrange
        var customer = GetValidCustomer();

        _countryRepositoryMock
            .Setup(x => x.ExistsAsync(customer.CountryId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.TestValidateAsync(customer);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateCustomerDTO.CountryId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Country.DoesNotExist));
    }

    [Test]
    public async Task ValidateAsync_ValidCustomer_NoErrorReturned()
    {
        // Arrange
        var customer = GetValidCustomer();

        // Act
        var result = await _validator.TestValidateAsync(customer);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Is.Empty);
    }

    private UpdateCustomerDTO GetValidCustomer()
    {
        var countryId = Guid.NewGuid();

        _countryRepositoryMock
            .Setup(x => x.ExistsAsync(countryId, CancellationToken.None))
        .ReturnsAsync(true);

        return Fixture.Build<UpdateCustomerDTO>()
            .With(x => x.CountryId, countryId)
            .Create();
    }
}
