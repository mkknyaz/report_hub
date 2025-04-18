using Exadel.ReportHub.SDK.DTOs.Item;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Item.Create;

public class CreateItemRequestValidator : AbstractValidator<CreateItemRequest>
{
    private readonly IValidator<CreateUpdateItemDTO> _itemValidator;

    public CreateItemRequestValidator(IValidator<CreateUpdateItemDTO> itemValidator)
    {
        _itemValidator = itemValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.CreateItemDto)
            .SetValidator(_itemValidator);
    }
}
