using System.Runtime.InteropServices;
using System.Security;
using Exadel.ReportHub.SDK.DTOs.User;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class PasswordValidator : AbstractValidator<string>
{
    public PasswordValidator()
    {
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .NotEmpty()
            .MinimumLength(Constants.Validation.User.PasswordMinimumLength)
            .Matches("[A-Z]")
            .WithMessage(Constants.Validation.User.PasswordUppercaseMessage)
            .Matches("[a-z]")
            .WithMessage(Constants.Validation.User.PasswordLowercaseMessage)
            .Matches("[0-9]")
            .WithMessage(Constants.Validation.User.PasswordDigitMessage)
            .Matches("[^a-zA-Z0-9]")
            .WithMessage(Constants.Validation.User.PasswordSpecialCharacterMessage)
            .WithName(nameof(CreateUserDTO.Password));
    }
}
