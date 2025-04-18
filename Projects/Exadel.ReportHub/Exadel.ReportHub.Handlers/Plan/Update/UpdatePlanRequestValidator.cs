using FluentValidation;

namespace Exadel.ReportHub.Handlers.Plan.Update;

public class UpdatePlanRequestValidator : AbstractValidator<UpdatePlanRequest>
{
    public UpdatePlanRequestValidator()
    {
        ConfigureRules();
    }

    public void ConfigureRules()
    {
        RuleFor(x => x.UpdatePlanDatedto)
            .ChildRules(child =>
            {
                child.RuleLevelCascadeMode = CascadeMode.Stop;

                child.RuleFor(x => x.Amount)
                    .GreaterThan(0);

                child.RuleFor(x => x.StartDate)
                    .NotEmpty()
                    .LessThan(x => x.EndDate)
                    .WithMessage(Constants.Validation.Plan.PlanStartDateErrorMessage);

                child.RuleFor(x => x.EndDate)
                    .NotEmpty()
                    .GreaterThan(DateTime.UtcNow)
                    .WithMessage(Constants.Validation.Plan.PlandEndDateInThePastErrorMessage);
            });
    }
}
