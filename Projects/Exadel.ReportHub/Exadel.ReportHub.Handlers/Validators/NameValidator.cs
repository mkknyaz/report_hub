using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class NameValidator : AbstractValidator<string>
{
    public NameValidator()
    {
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .NotEmpty()
            .MaximumLength(Constants.Validation.Customer.NameMaxLength)
            .Matches("^[A-Z]")
            .WithMessage(Constants.Validation.Customer.NameShouldStartWithCapitalMessage)
            .WithName(nameof(Data.Models.Customer.Name));
    }
}
