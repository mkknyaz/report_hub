using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Plan;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Plan.Create;

public class CreatePlanRequestValidator : AbstractValidator<CreatePlanRequest>
{
    private readonly IValidator<CreatePlanDTO> _createPlanDtoValidator;

    public CreatePlanRequestValidator(IValidator<CreatePlanDTO> createPlanDtoValidator)
    {
        _createPlanDtoValidator = createPlanDtoValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.CreatePlanDto)
            .SetValidator(_createPlanDtoValidator);
    }
}
