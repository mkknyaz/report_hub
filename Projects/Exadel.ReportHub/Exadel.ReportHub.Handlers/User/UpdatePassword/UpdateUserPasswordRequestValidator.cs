using FluentValidation;

namespace Exadel.ReportHub.Handlers.User.UpdatePassword;

public class UpdateUserPasswordRequestValidator : AbstractValidator<UpdateUserPasswordRequest>
{
    private readonly IValidator<string> _passwordValidator;

    public UpdateUserPasswordRequestValidator(IValidator<string> passwordValidator)
    {
        _passwordValidator = passwordValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.Password)
            .SetValidator(_passwordValidator);
    }
}
