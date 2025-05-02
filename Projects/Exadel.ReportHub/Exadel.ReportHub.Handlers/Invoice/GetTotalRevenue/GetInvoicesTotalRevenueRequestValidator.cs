using FluentValidation;

namespace Exadel.ReportHub.Handlers.Invoice.GetTotalRevenue;

public class GetInvoicesTotalRevenueRequestValidator : AbstractValidator<GetInvoicesTotalRevenueRequest>
{
    public GetInvoicesTotalRevenueRequestValidator()
    {
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.InvoiceDateFilterDto)
            .ChildRules(child =>
            {
                RuleLevelCascadeMode = CascadeMode.Stop;

                child.RuleFor(x => x.StartDate)
                    .NotEmpty()
                    .LessThan(x => x.EndDate)
                    .WithMessage(Constants.Validation.Date.InvalidStartDate);

                child.RuleFor(x => x.StartDate.TimeOfDay)
                    .Equal(TimeSpan.Zero)
                    .WithMessage(Constants.Validation.Invoice.TimeComponentNotAllowed);

                child.RuleFor(x => x.EndDate)
                    .NotEmpty();

                child.RuleFor(x => x.EndDate.TimeOfDay)
                    .Equal(TimeSpan.Zero)
                    .WithMessage(Constants.Validation.Invoice.TimeComponentNotAllowed);
            });
    }
}
