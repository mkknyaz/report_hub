using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Item.Update;
using Exadel.ReportHub.SDK.DTOs.Item;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;

namespace Exadel.ReportHub.Tests.Handlers.Item.Update;

[TestFixture]
public class UpdateItemRequestValidatorTests
{
    private UpdateItemRequestValidator _validator;

    [SetUp]
    public void Setup()
    {
        var itemValidatorMock = new InlineValidator<CreateUpdateItemDTO>();
        itemValidatorMock.RuleSet("Default", () =>
        {
            itemValidatorMock.RuleLevelCascadeMode = CascadeMode.Stop;

            itemValidatorMock.RuleFor(x => x.Price)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage(Constants.Validation.Item.PriceMustBePositive);
        });
        _validator = new UpdateItemRequestValidator(itemValidatorMock);
    }

    [Test]
    public async Task ValidateAsync_ValidRequest_NoErrorsReturned()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var updateItemDto = new CreateUpdateItemDTO { Price = 1500.50m };

        // Act
        var request = new UpdateItemRequest(itemId, updateItemDto);
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task ValidateAsync_EmptyItemId_ErrorReturned()
    {
        // Arrange
        var itemId = Guid.Empty;
        var updateItemDto = new CreateUpdateItemDTO { Price = 1500.50m };

        // Act
        var request = new UpdateItemRequest(itemId, updateItemDto);
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("'Id' must not be empty.");
    }

    [Test]
    public async Task ValidateAsync_InvalidItemDto_ErrorReturned()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var updateItemDto = new CreateUpdateItemDTO { Price = -1500.50m };

        // Act
        var request = new UpdateItemRequest(itemId, updateItemDto);
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateItemDto.Price)
            .WithErrorMessage(Constants.Validation.Item.PriceMustBePositive);
    }
}
