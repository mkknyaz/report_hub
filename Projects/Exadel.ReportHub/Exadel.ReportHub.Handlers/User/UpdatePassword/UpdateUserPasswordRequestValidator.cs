using FluentValidation;

namespace Exadel.ReportHub.Handlers.User.UpdatePassword;

public class UpdateUserPasswordRequestValidator : AbstractValidator<UpdateUserPasswordRequest>
{
    private readonly IValidator<string> _stringValidator;

    public UpdateUserPasswordRequestValidator(IValidator<string> stringValidator)
    {
        _stringValidator = stringValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.Password)
            .SetValidator(_stringValidator, Constants.Validation.RuleSet.Passwords);
    }
}
