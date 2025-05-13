using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Client;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class CreateClientValidator : AbstractValidator<CreateClientDTO>
{
    private readonly IClientRepository _clientRepository;
    private readonly ICountryRepository _countryRepository;
    private readonly IValidator<string> _stringValidator;

    public CreateClientValidator(ICountryRepository countryRepository, IClientRepository clientRepository, IValidator<string> stringValidator)
    {
        _countryRepository = countryRepository;
        _clientRepository = clientRepository;
        _stringValidator = stringValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name)
            .SetValidator(_stringValidator, Constants.Validation.RuleSet.Names)
            .MustAsync(async (name, cancellationToken) => !await _clientRepository.NameExistsAsync(name, cancellationToken))
            .WithMessage(Constants.Validation.Name.IsTaken);
        RuleFor(x => x.BankAccountNumber)
            .NotEmpty()
            .Length(Constants.Validation.BankAccountNumber.MinLength, Constants.Validation.BankAccountNumber.MaxLength)
            .Matches(@"^[A-Z]{2}\d+$")
            .WithMessage(Constants.Validation.BankAccountNumber.InvalidFormat)
            .MustAsync(ValidateBankAccountNumberAsync)
            .WithMessage(Constants.Validation.BankAccountNumber.InvalidCountryCode);
        RuleFor(x => x.CountryId)
            .NotEmpty()
            .MustAsync(_countryRepository.ExistsAsync)
            .WithMessage(Constants.Validation.Country.DoesNotExist);
    }

    private async Task<bool> ValidateBankAccountNumberAsync(string bankAccountNumber, CancellationToken cancellationToken)
    {
        var countryCode = bankAccountNumber.Substring(0, 2);
        return await _countryRepository.CountryCodeExistsAsync(countryCode, cancellationToken);
    }
}
