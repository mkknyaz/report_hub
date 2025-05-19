using Exadel.ReportHub.Handlers;
using Exadel.ReportHub.Handlers.Item.Create;
using Exadel.ReportHub.SDK.DTOs.Item;
using FluentValidation;
using FluentValidation.TestHelper;

namespace Exadel.ReportHub.Tests.Handlers.Item.Create;

[TestFixture]
public class CreateItemRequestValidatorTests
{
    private CreateItemRequestValidator _validator;

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
        _validator = new CreateItemRequestValidator(itemValidatorMock);
    }

    [Test]
    public async Task ValidateAsync_ValidRequest_NoErrorsReturned()
    {
        // Arrange
        var createItemDto = new CreateUpdateItemDTO { Price = 1500.50m };

        // Act
        var request = new CreateItemRequest(createItemDto);
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task ValidateAsync_InvalidItemDto_ErrorReturned()
    {
        // Arrange
        var createItemDto = new CreateUpdateItemDTO { Price = -1500.50m };

        // Act
        var request = new CreateItemRequest(createItemDto);
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateItemDto.Price)
            .WithErrorMessage(Constants.Validation.Item.PriceMustBePositive);
    }
}
