using Exadel.ReportHub.Handlers.User.UpdateName;
using Exadel.ReportHub.SDK.DTOs.User;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class UserNameValidator : AbstractValidator<string>
{
    public UserNameValidator()
    {
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .NotEmpty()
            .MaximumLength(Constants.Validation.User.FullNameMaxLength)
            .WithName(nameof(CreateUserDTO.FullName));
    }
}
