using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class StringValidator : AbstractValidator<string>
{
    public StringValidator()
    {
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleSet(Constants.Validation.RuleSet.Names, () =>
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .NotEmpty()
                .MaximumLength(Constants.Validation.Name.MaxLength)
                .Matches("^[A-Z]")
                .WithMessage(Constants.Validation.Name.MustStartWithCapital)
                .WithName(nameof(Constants.Validation.Name));
        });

        RuleSet(Constants.Validation.RuleSet.Passwords, () =>
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .NotEmpty()
                .MinimumLength(Constants.Validation.Password.MinimumLength)
                .Matches("[A-Z]")
                .WithMessage(Constants.Validation.Password.RequireUppercase)
                .Matches("[a-z]")
                .WithMessage(Constants.Validation.Password.RequireLowercase)
                .Matches("[0-9]")
                .WithMessage(Constants.Validation.Password.RequireDigit)
                .Matches("[^a-zA-Z0-9]")
                .WithMessage(Constants.Validation.Password.RequireSpecialCharacter)
                .WithName(nameof(Constants.Validation.Password));
        });

        RuleSet(Constants.Validation.RuleSet.Countries, () =>
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .NotEmpty()
                .MaximumLength(Constants.Validation.Country.MaxLength)
                .Matches("^[A-Z]")
                .WithMessage(Constants.Validation.Country.MustStartWithCapital)
                .WithName(nameof(Constants.Validation.Country));
        });
    }
}
