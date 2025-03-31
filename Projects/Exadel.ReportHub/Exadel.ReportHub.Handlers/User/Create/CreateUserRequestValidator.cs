using Exadel.ReportHub.RA.Abstract;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.User.Create;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    private readonly IUserRepository _userRepository;

    public CreateUserRequestValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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
                    .WithMessage(Constants.Validation.User.EmailInvalidMessage)
                    .MustAsync(EmailMustNotExistAsync)
                    .WithMessage(Constants.Validation.User.EmailTakenMessage);

                child.RuleFor(x => x.FullName)
                    .NotEmpty();

                child.RuleFor(x => x.Password.ToString())
                    .NotEmpty()
                    .MinimumLength(Constants.Validation.User.PasswordMinimumLength)
                    .WithMessage(Constants.Validation.User.PasswordMinLengthMessage)
                    .Matches("[A-Z]")
                    .WithMessage(Constants.Validation.User.PasswordUppercaseMessage)
                    .Matches("[a-z]")
                    .WithMessage(Constants.Validation.User.PasswordLowercaseMessage)
                    .Matches("[0-9]")
                    .WithMessage(Constants.Validation.User.PasswordDigitMessage)
                    .Matches("[^a-zA-Z0-9]")
                    .WithMessage(Constants.Validation.User.PasswordSpecialCharacterMessage);
            });
    }

    private async Task<bool> EmailMustNotExistAsync(string email, CancellationToken cancellationToken)
    {
        var emailExists = await _userRepository.EmailExistsAsync(email, cancellationToken);
        return !emailExists;
    }
}
