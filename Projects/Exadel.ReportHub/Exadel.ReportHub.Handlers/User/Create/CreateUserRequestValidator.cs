using Exadel.ReportHub.RA.Abstract;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.User.Create;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<string> _stringValidator;

    public CreateUserRequestValidator(IUserRepository userRepository, IValidator<string> stringValidator)
    {
        _userRepository = userRepository;
        _stringValidator = stringValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.CreateUserDto)
            .ChildRules(child =>
            {
                child.RuleLevelCascadeMode = CascadeMode.Stop;

                child.RuleFor(x => x.Email)
                    .NotEmpty()
                    .EmailAddress()
                    .WithMessage(Constants.Validation.Email.IsInvalid)
                    .MustAsync(EmailMustNotExistAsync)
                    .WithMessage(Constants.Validation.Email.IsTaken);

                child.RuleFor(x => x.FullName)
                    .SetValidator(_stringValidator, Constants.Validation.RuleSet.Names);

                child.RuleFor(x => x.Password)
                    .SetValidator(_stringValidator, Constants.Validation.RuleSet.Passwords);
            });
    }

    private async Task<bool> EmailMustNotExistAsync(string email, CancellationToken cancellationToken)
    {
        var emailExists = await _userRepository.EmailExistsAsync(email, cancellationToken);
        return !emailExists;
    }
}
