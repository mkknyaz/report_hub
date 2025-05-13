using Exadel.ReportHub.SDK.DTOs.Plan;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Plan.Update;

public class UpdatePlanRequestValidator : AbstractValidator<UpdatePlanRequest>
{
    private readonly IValidator<UpdatePlanDTO> _updatePlanDtoValidator;

    public UpdatePlanRequestValidator(IValidator<UpdatePlanDTO> updatePlanDtoValidator)
    {
        _updatePlanDtoValidator = updatePlanDtoValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.UpdatePlanDto)
            .SetValidator(_updatePlanDtoValidator);
    }
}
