using FluentValidation;

namespace Exadel.ReportHub.Handlers.User.UpdateName;

public class UpdateUserNameRequestValidator : AbstractValidator<UpdateUserNameRequest>
{
    private readonly IValidator<string> _userNameValidator;

    public UpdateUserNameRequestValidator(IValidator<string> userNameValidator)
    {
        _userNameValidator = userNameValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.FullName)
            .SetValidator(_userNameValidator);
    }
}
