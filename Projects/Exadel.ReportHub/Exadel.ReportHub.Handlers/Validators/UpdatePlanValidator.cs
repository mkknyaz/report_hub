using Exadel.ReportHub.SDK.DTOs.Plan;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class UpdatePlanValidator : AbstractValidator<UpdatePlanDTO>
{
    public UpdatePlanValidator()
    {
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Count)
            .GreaterThan(0);

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .LessThan(x => x.EndDate)
            .WithMessage(Constants.Validation.Date.InvalidStartDate);

        RuleFor(x => x.EndDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage(Constants.Validation.Date.EndDateInPast);
    }
}
