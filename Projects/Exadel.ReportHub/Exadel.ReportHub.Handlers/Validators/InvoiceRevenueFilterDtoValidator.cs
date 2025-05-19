using Exadel.ReportHub.SDK.DTOs.Invoice;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class InvoiceRevenueFilterDtoValidator : AbstractValidator<InvoiceRevenueFilterDTO>
{
    public InvoiceRevenueFilterDtoValidator()
    {
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x)
            .ChildRules(child =>
            {
                RuleLevelCascadeMode = CascadeMode.Stop;
                ClassLevelCascadeMode = CascadeMode.Stop;

                child.RuleFor(x => x.StartDate)
                    .NotEmpty()
                    .WithMessage(Constants.Validation.Date.EmptyStartDate)
                    .LessThanOrEqualTo(x => x.EndDate)
                    .WithMessage(Constants.Validation.Date.InvalidStartDate);

                child.RuleFor(x => x.EndDate)
                    .LessThanOrEqualTo(DateTime.UtcNow)
                    .WithMessage(Constants.Validation.Date.EndDateNotInPast);

                child.RuleFor(x => x.StartDate.TimeOfDay)
                    .Equal(TimeSpan.Zero)
                    .WithMessage(Constants.Validation.Date.TimeComponentNotAllowed);

                child.RuleFor(x => x.EndDate.TimeOfDay)
                    .Equal(TimeSpan.Zero)
                    .WithMessage(Constants.Validation.Date.TimeComponentNotAllowed);
            });
    }
}
