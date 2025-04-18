using Exadel.ReportHub.SDK.DTOs.Item;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Item.Update;

public class UpdateItemRequestValidator : AbstractValidator<UpdateItemRequest>
{
    private readonly IValidator<CreateUpdateItemDTO> _itemValidator;

    public UpdateItemRequestValidator(IValidator<CreateUpdateItemDTO> itemValidator)
    {
        _itemValidator = itemValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.UpdateItemDTO)
            .SetValidator(_itemValidator);
    }
}
