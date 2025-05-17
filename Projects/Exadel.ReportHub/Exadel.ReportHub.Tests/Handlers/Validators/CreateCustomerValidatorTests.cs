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
public class CreateCustomerValidatorTests : BaseTestFixture
{
    private CreateCustomerValidator _validator;
    private Mock<ICustomerRepository> _customerRepositoryMock;
    private Mock<IClientRepository> _clientRepositoryMock;

    [SetUp]
    public void Setup()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        var importCustomerDtoValidator = new InlineValidator<ImportCustomerDTO>();
        importCustomerDtoValidator.RuleSet("Default", () =>
        {
            importCustomerDtoValidator.RuleLevelCascadeMode = CascadeMode.Stop;

            importCustomerDtoValidator.RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage(Constants.Validation.Email.IsInvalid)
                .MustAsync(async (email, cancellationToken) => !await _customerRepositoryMock.Object.EmailExistsAsync(email, cancellationToken))
                .WithMessage(Constants.Validation.Email.IsTaken);
        });
        _clientRepositoryMock = new Mock<IClientRepository>();
        _validator = new CreateCustomerValidator(_clientRepositoryMock.Object, importCustomerDtoValidator);
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
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateCustomerDTO.Email)));
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
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateCustomerDTO.Email)));
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
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateCustomerDTO.Email)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Email.IsTaken));
    }

    [Test]
    public async Task ValidateAsync_EmptyCliendId_ErrorReturned()
    {
        // Arrange
        var customer = SetupValidCustomer();
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
        var customer = SetupValidCustomer();

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

    private CreateCustomerDTO SetupValidCustomer()
    {
        var email = "customer@test.com";
        var customer = Fixture.Build<CreateCustomerDTO>()
            .With(x => x.Email, email)
            .Create();

        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(customer.ClientId, CancellationToken.None))
            .ReturnsAsync(true);

        _customerRepositoryMock
            .Setup(x => x.EmailExistsAsync(customer.Email, CancellationToken.None))
            .ReturnsAsync(false);

        return customer;
    }
}
