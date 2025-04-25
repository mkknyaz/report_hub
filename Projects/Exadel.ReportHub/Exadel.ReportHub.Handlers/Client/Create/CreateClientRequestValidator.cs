using Exadel.ReportHub.RA.Abstract;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Client.Create;

public class CreateClientRequestValidator : AbstractValidator<CreateClientRequest>
{
    private readonly IClientRepository _clientRepository;
    private readonly IValidator<string> _stringValidator;

    public CreateClientRequestValidator(IClientRepository clientRepository, IValidator<string> stringValidator)
    {
        _clientRepository = clientRepository;
        _stringValidator = stringValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.CreateClientDto)
            .NotEmpty()
            .ChildRules(child =>
            {
                child.RuleFor(x => x.Name)
                    .SetValidator(_stringValidator, Constants.Validation.RuleSet.Names)
                    .MustAsync(async (name, cancellationToken) => !await _clientRepository.NameExistsAsync(name, cancellationToken))
                    .WithMessage(Constants.Validation.Name.AlreadyTaken);
                child.RuleFor(x => x.BankAccountNumber)
                    .NotEmpty()
                    .Length(Constants.Validation.BankAccountNumber.MinLength, Constants.Validation.BankAccountNumber.MaxLength)
                    .Matches(@"^[A-Z]{2}\d+$")
                    .WithMessage(Constants.Validation.BankAccountNumber.InvalidFormat);
            });
    }
}
