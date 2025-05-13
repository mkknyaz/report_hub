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

[TestFixture]
public class CreateCustomerValidatorTests : BaseTestFixture
{
    private CreateCustomerValidator _validator;
    private Mock<ICustomerRepository> _customerRepositoryMock;
    private Mock<IClientRepository> _clientRepositoryMock;
    private Mock<ICountryRepository> _countryRepositoryMock;

    [SetUp]
    public void Setup()
    {
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
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _validator = new CreateCustomerValidator(_customerRepositoryMock.Object, _clientRepositoryMock.Object, updateCustomerDtoValidator);
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

    [Test]
    [TestCase("")]
    [TestCase(null)]
    public async Task ValidateAsync_EmptyEmail_ErrorReturned(string email)
    {
        // Arrange
        var customer = GetValidCustomer();
        customer.Email = email;

        // Act
        var result = await _validator.TestValidateAsync(customer);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateCustomerDTO.Email)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo($"'{nameof(customer.Email)}' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_InvalidEmailFormat_ErrorReturned()
    {
        // Arrange
        var customer = GetValidCustomer();
        customer.Email = "customer";

        // Act
        var result = await _validator.TestValidateAsync(customer);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateCustomerDTO.Email)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Email.IsInvalid));
    }

    [Test]
    public async Task ValidateAsync_EmailIsTaken_ErrorReturned()
    {
        // Arrange
        var customer = GetValidCustomer();

        _customerRepositoryMock.Setup(x => x.EmailExistsAsync(customer.Email, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var result = await _validator.TestValidateAsync(customer);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateCustomerDTO.Email)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Email.IsTaken));
    }

    [Test]
    public async Task ValidateAsync_EmptyCliendId_ErrorReturned()
    {
        // Arrange
        var customer = GetValidCustomer();
        customer.ClientId = Guid.Empty;

        // Act
        var result = await _validator.TestValidateAsync(customer);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateCustomerDTO.ClientId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Client Id' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_ClientIdDoesNotExist_ErrorReturned()
    {
        // Arrange
        var customer = GetValidCustomer();
        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(customer.ClientId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.TestValidateAsync(customer);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateCustomerDTO.ClientId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Client.DoesNotExist));
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

    private CreateCustomerDTO GetValidCustomer()
    {
        var email = "customer@test.com";
        var clientId = Guid.NewGuid();
        var countryId = Guid.NewGuid();

        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(clientId, CancellationToken.None))
            .ReturnsAsync(true);

        _countryRepositoryMock
            .Setup(x => x.ExistsAsync(countryId, CancellationToken.None))
        .ReturnsAsync(true);

        _customerRepositoryMock
            .Setup(x => x.EmailExistsAsync(email, CancellationToken.None))
            .ReturnsAsync(false);

        return Fixture.Build<CreateCustomerDTO>()
            .With(x => x.Email, email)
            .With(x => x.ClientId, clientId)
            .With(x => x.CountryId, countryId)
            .Create();
    }
}
