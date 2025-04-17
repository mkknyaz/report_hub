using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class CountryValidator : AbstractValidator<string>
{
    public CountryValidator()
    {
        ConfigureRules();
    }

    public void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .NotEmpty()
            .Matches("^[A-Z]")
            .WithMessage(Constants.Validation.Customer.CountryShouldStartWithCapitalMessage)
            .MaximumLength(Constants.Validation.Customer.CountryMaxLength);
    }
}
