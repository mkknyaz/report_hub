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
                .WithMessage(Constants.Validation.Name.ShouldStartWithCapitalMessage)
                .WithName(nameof(Constants.Validation.Name));
        });

        RuleSet(Constants.Validation.RuleSet.Passwords, () =>
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .NotEmpty()
                .MinimumLength(Constants.Validation.Password.MinimumLength)
                .Matches("[A-Z]")
                .WithMessage(Constants.Validation.Password.UppercaseMessage)
                .Matches("[a-z]")
                .WithMessage(Constants.Validation.Password.LowercaseMessage)
                .Matches("[0-9]")
                .WithMessage(Constants.Validation.Password.DigitMessage)
                .Matches("[^a-zA-Z0-9]")
                .WithMessage(Constants.Validation.Password.SpecialCharacterMessage)
                .WithName(nameof(Constants.Validation.Password));
        });

        RuleSet(Constants.Validation.RuleSet.Countries, () =>
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x)
                .NotEmpty()
                .MaximumLength(Constants.Validation.Country.MaxLength)
                .Matches("^[A-Z]")
                .WithMessage(Constants.Validation.Country.ShouldStartWithCapitalMessage)
                .WithName(nameof(Constants.Validation.Country));
        });
    }
}
