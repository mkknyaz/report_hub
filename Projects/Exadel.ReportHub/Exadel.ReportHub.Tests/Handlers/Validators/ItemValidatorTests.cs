using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Item;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Validators;

[TestFixture]
public class ItemValidatorTests : BaseTestFixture
{
    private Mock<ICurrencyRepository> _currencyRepositoryMock;
    private Mock<IClientRepository> _clientRepositoryMock;
    private ItemValidator _validator;

    [SetUp]
    public void Setup()
    {
        _currencyRepositoryMock = new Mock<ICurrencyRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();

        var stringValidator = new InlineValidator<string>();
        stringValidator.RuleSet(Constants.Validation.RuleSet.Names, () =>
        {
            stringValidator.RuleFor(x => x)
                .NotEmpty()
                .MaximumLength(Constants.Validation.Name.MaxLength)
                .WithName(nameof(Constants.Validation.Name));
        });

        _validator = new ItemValidator(
            _currencyRepositoryMock.Object,
            _clientRepositoryMock.Object,
            stringValidator);
    }

    [Test]
    public async Task ValidateAsync_ValidItem_NoErrorReturned()
    {
        // Arrange
        var item = SetupValidItem();

        // Act
        var result = await _validator.TestValidateAsync(item);

        // Assert
        Assert.That(result.IsValid, Is.True);
        Assert.That(result.Errors, Is.Empty);
    }

    [Test]
    public async Task ValidateAsync_EmptyClientId_ErrorReturned()
    {
        // Arrange
        var item = SetupValidItem();
        item.ClientId = Guid.Empty;

        // Act
        var result = await _validator.TestValidateAsync(item);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateUpdateItemDTO.ClientId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Client Id' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_ClientDoesNotExist_ErrorReturned()
    {
        // Arrange
        var item = SetupValidItem();

        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(item.ClientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.TestValidateAsync(item);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateUpdateItemDTO.ClientId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Client.DoesNotExist));
    }

    [Test]
    public async Task ValidateAsync_EmptyCurrencyId_ErrorReturned()
    {
        // Arrange
        var item = SetupValidItem();
        item.CurrencyId = Guid.Empty;

        // Act
        var result = await _validator.TestValidateAsync(item);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateUpdateItemDTO.CurrencyId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Currency Id' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_CurrencyDoesNotExist_ErrorReturned()
    {
        // Arrange
        var item = SetupValidItem();

        _currencyRepositoryMock
            .Setup(x => x.ExistsAsync(item.CurrencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _validator.TestValidateAsync(item);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateUpdateItemDTO.CurrencyId)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Currency.DoesNotExist));
    }

    [Test]
    [TestCase("")]
    [TestCase(null)]
    public async Task ValidateAsync_EmptyDescription_ErrorReturned(string description)
    {
        // Arrange
        var item = SetupValidItem();
        item.Description = description;

        // Act
        var result = await _validator.TestValidateAsync(item);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateUpdateItemDTO.Description)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Description' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_DescriptionTooLong_ErrorReturned()
    {
        // Arrange
        var item = SetupValidItem();
        item.Description = new string('x', Constants.Validation.Item.DescriptionMaxLength + 1);

        // Act
        var result = await _validator.TestValidateAsync(item);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateUpdateItemDTO.Description)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo($"The length of 'Description' must be " +
            $"{Constants.Validation.Item.DescriptionMaxLength} characters or fewer. You entered {item.Description.Length} characters."));
    }

    [Test]
    public async Task ValidateAsync_DescriptionNotStartingWithCapital_ErrorReturned()
    {
        // Arrange
        var item = SetupValidItem();
        item.Description = "invalid description";

        // Act
        var result = await _validator.TestValidateAsync(item);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateUpdateItemDTO.Description)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Item.DescriptionShouldStartWithCapital));
    }

    [Test]
    public async Task ValidateAsync_EmptyPrice_ErrorReturned()
    {
        // Arrange
        var item = SetupValidItem();
        item.Price = 0m;

        // Act
        var result = await _validator.TestValidateAsync(item);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateUpdateItemDTO.Price)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo("'Price' must not be empty."));
    }

    [Test]
    public async Task ValidateAsync_NegativePrice_ErrorReturned()
    {
        // Arrange
        var item = SetupValidItem();
        item.Price = -1m;

        // Act
        var result = await _validator.TestValidateAsync(item);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateUpdateItemDTO.Price)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Item.PriceMustBePositive));
    }

    private CreateUpdateItemDTO SetupValidItem()
    {
        var item = Fixture.Create<CreateUpdateItemDTO>();

        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(item.ClientId, CancellationToken.None))
            .ReturnsAsync(true);

        _currencyRepositoryMock
            .Setup(x => x.ExistsAsync(item.CurrencyId, CancellationToken.None))
            .ReturnsAsync(true);

        return item;
    }
}
