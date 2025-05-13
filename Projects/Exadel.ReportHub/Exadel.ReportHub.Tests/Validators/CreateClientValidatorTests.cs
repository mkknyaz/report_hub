using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Client;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;

namespace Exadel.ReportHub.Tests.Validators;

[TestFixture]
public class CreateClientValidatorTests : BaseTestFixture
{
    private CreateClientValidator _validator;
    private Mock<IClientRepository> _clientRepositoryMock;
    private Mock<ICountryRepository> _countryRepositoryMock;

    [SetUp]
    public void Setup()
    {
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
        _clientRepositoryMock = new Mock<IClientRepository>();
        _countryRepositoryMock = new Mock<ICountryRepository>();
        _validator = new CreateClientValidator(_countryRepositoryMock.Object, _clientRepositoryMock.Object, stringValidator);
    }

    [Test]
    public async Task ValidateAsync_ValidClient_NoErrorReturned()
    {
        // Arrange
        var client = GetValidClient();

        // Act
        var result = await _validator.TestValidateAsync(client);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Is.Empty);
    }

    [Test]
    [TestCase("")]
    [TestCase(null)]
    public async Task ValidateAsync_EmptyBankAccountNumber_ErrorReturned(string bankAccountNumber)
    {
        // Arrange
        var client = GetValidClient();
        client.BankAccountNumber = bankAccountNumber;

        // Act
        var result = await _validator.TestValidateAsync(client);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateClientDTO.BankAccountNumber)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Bank Account Number' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_InvalidCountryCode_ErrorReturned()
    {
        // Arrange
        var client = GetValidClient();
        client.BankAccountNumber = "ZZ123456778";
        _countryRepositoryMock
            .Setup(x => x.CountryCodeExistsAsync(client.BankAccountNumber.Substring(0, 2), CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.TestValidateAsync(client);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateClientDTO.BankAccountNumber)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.BankAccountNumber.InvalidCountryCode));
    }

    [Test]
    [TestCase("PL123")]
    [TestCase("PL1234567890123456789012345644324343343434")]
    public async Task ValidateAsync_BankAccountNumberIsNotInValidRange_ErrorReturned(string bankAccountNumber)
    {
        // Arrange
        var client = GetValidClient();
        client.BankAccountNumber = bankAccountNumber;

        // Act
        var result = await _validator.TestValidateAsync(client);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateClientDTO.BankAccountNumber)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo($"'Bank Account Number' must be between " +
            $"{Constants.Validation.BankAccountNumber.MinLength} and {Constants.Validation.BankAccountNumber.MaxLength} characters. You entered {client.BankAccountNumber.Length} characters."));
    }

    [Test]
    public async Task ValidateAsync_BankAccountNumberIsNotStartingWithTwoLetters_ErrorReturned()
    {
        // Arrange
        var client = GetValidClient();
        client.BankAccountNumber = "1234567890";

        // Act
        var result = await _validator.TestValidateAsync(client);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateClientDTO.BankAccountNumber)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.BankAccountNumber.InvalidFormat));
    }

    [Test]
    public async Task ValidateAsync_CountryDoesNotExist_ErrorReturned()
    {
        // Arrange
        var client = GetValidClient();
        client.CountryId = Guid.NewGuid();

        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(client.CountryId, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.TestValidateAsync(client);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateClientDTO.CountryId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Country.DoesNotExist));
    }

    [Test]
    public async Task ValidateAsync_EmptyCountryId_ErrorReturned()
    {
        // Arrange
        var client = GetValidClient();
        client.CountryId = Guid.Empty;

        // Act
        var result = await _validator.TestValidateAsync(client);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateClientDTO.CountryId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Country Id' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_NameExists_ErrorReturned()
    {
        // Arrange
        var client = GetValidClient();
        client.Name = "Microsoft Inc.";

        _clientRepositoryMock
            .Setup(x => x.NameExistsAsync(client.Name, CancellationToken.None))
            .ReturnsAsync(true);

        // Act
        var result = await _validator.TestValidateAsync(client);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateClientDTO.Name)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Name.IsTaken));
    }

    private CreateClientDTO GetValidClient()
    {
        var name = "Organization Inc.";
        var bankAccountNumber = "US1234567890";
        var countryId = Guid.NewGuid();

        _clientRepositoryMock
            .Setup(x => x.NameExistsAsync(name, CancellationToken.None))
            .ReturnsAsync(false);

        _countryRepositoryMock
            .Setup(x => x.ExistsAsync(countryId, CancellationToken.None))
        .ReturnsAsync(true);

        _countryRepositoryMock
            .Setup(x => x.CountryCodeExistsAsync(bankAccountNumber.Substring(0, 2), CancellationToken.None))
            .ReturnsAsync(true);

        return Fixture.Build<CreateClientDTO>()
            .With(x => x.Name, name)
            .With(x => x.BankAccountNumber, bankAccountNumber)
            .With(x => x.CountryId, countryId)
            .Create();
    }
}
