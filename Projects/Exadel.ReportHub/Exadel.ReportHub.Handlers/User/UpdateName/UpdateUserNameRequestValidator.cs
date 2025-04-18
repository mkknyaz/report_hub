using FluentValidation;

namespace Exadel.ReportHub.Handlers.User.UpdateName;

public class UpdateUserNameRequestValidator : AbstractValidator<UpdateUserNameRequest>
{
    private readonly IValidator<string> _stringValidator;

    public UpdateUserNameRequestValidator(IValidator<string> stringValidator)
    {
        _stringValidator = stringValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.FullName)
            .SetValidator(_stringValidator, Constants.Validation.RuleSet.Names);
    }
}
