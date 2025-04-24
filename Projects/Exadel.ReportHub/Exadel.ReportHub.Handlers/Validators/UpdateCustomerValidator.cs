using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerDTO>
{
    private readonly ICountryRepository _countryRepository;
    private readonly IValidator<string> _stringValidator;

    public UpdateCustomerValidator(ICountryRepository countryRepository, IValidator<string> stringValidator)
    {
        _countryRepository = countryRepository;
        _stringValidator = stringValidator;
        ConfigureRules();
    }

    public void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name)
            .SetValidator(_stringValidator, Constants.Validation.RuleSet.Names);

        RuleFor(x => x.CountryId)
            .NotEmpty()
            .MustAsync(_countryRepository.ExistsAsync)
            .WithMessage(Constants.Validation.Customer.CountryDoesNotExistMessage);
    }
}
