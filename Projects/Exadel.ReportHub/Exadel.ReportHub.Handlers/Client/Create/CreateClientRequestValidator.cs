using Exadel.ReportHub.RA.Abstract;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Client.Create;

public class CreateClientRequestValidator : AbstractValidator<CreateClientRequest>
{
    private readonly IClientRepository _clientRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IValidator<string> _stringValidator;

    public CreateClientRequestValidator(ICountryRepository countryRepository, IClientRepository clientRepository, IValidator<string> stringValidator)
    {
        _countryRepository = countryRepository;
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
                child.RuleLevelCascadeMode = CascadeMode.Stop;

                child.RuleFor(x => x.Name)
                    .SetValidator(_stringValidator, Constants.Validation.RuleSet.Names)
                    .MustAsync(async (name, cancellationToken) => !await _clientRepository.NameExistsAsync(name, cancellationToken))
                    .WithMessage(Constants.Validation.Name.AlreadyTaken);
                child.RuleFor(x => x.BankAccountNumber)
                    .NotEmpty()
                    .Length(Constants.Validation.BankAccountNumber.MinLength, Constants.Validation.BankAccountNumber.MaxLength)
                    .Matches(@"^[A-Z]{2}\d+$")
                    .WithMessage(Constants.Validation.BankAccountNumber.InvalidFormat)
                    .MustAsync(ValidateBankAccountNumberAsync)
                    .WithMessage(Constants.Validation.BankAccountNumber.InvalidCountryCode);
                child.RuleFor(x => x.CountryId)
                    .NotEmpty()
                    .MustAsync(_countryRepository.ExistsAsync)
                    .WithMessage(Constants.Validation.Country.DoesNotExist);
            });
    }

    private async Task<bool> ValidateBankAccountNumberAsync(string bankAccountNumber, CancellationToken cancellationToken)
    {
        var countryCode = bankAccountNumber.Substring(0, 2);
        return await _countryRepository.CountryCodeExistsAsync(countryCode, cancellationToken);
    }
}
