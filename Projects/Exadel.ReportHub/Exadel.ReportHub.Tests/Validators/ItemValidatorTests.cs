using AutoFixture;
using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Validators;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Item;
using Exadel.ReportHub.Tests.Abstracts;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;

namespace Exadel.ReportHub.Tests.Validators;

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
        var item = GetValidItem();

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
        var item = GetValidItem();
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
        var item = GetValidItem();
        item.ClientId = Guid.NewGuid();

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
        var item = GetValidItem();
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
        var item = GetValidItem();
        item.CurrencyId = Guid.NewGuid();

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
        var item = GetValidItem();
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
        var item = GetValidItem();
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
        var item = GetValidItem();
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
        var item = GetValidItem();
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
        var item = GetValidItem();
        item.Price = -1m;

        // Act
        var result = await _validator.TestValidateAsync(item);

        // Assert
        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Items);
        Assert.That(result.Errors[0].PropertyName, Is.EqualTo(nameof(CreateUpdateItemDTO.Price)));
        Assert.That(result.Errors[0].ErrorMessage, Is.EqualTo(Constants.Validation.Item.PriceMustBePositive));
    }

    private CreateUpdateItemDTO GetValidItem()
    {
        var clientId = Guid.Parse("ea94747b-3d45-46d6-8775-bf27eb5da02b");
        var currencyId = Guid.Parse("04d123f0-dc7e-4b92-829c-dffd1ef0b89a");

        _clientRepositoryMock
            .Setup(x => x.ExistsAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _currencyRepositoryMock
            .Setup(x => x.ExistsAsync(currencyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        return Fixture.Build<CreateUpdateItemDTO>()
            .With(x => x.ClientId, clientId)
            .With(x => x.CurrencyId, currencyId)
            .Create();
    }
}
