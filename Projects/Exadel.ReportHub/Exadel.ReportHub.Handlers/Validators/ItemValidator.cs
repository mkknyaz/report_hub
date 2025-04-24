using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Item;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class ItemValidator : AbstractValidator<CreateUpdateItemDTO>
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IValidator<string> _stringValidator;

    public ItemValidator(ICurrencyRepository currencyRepository, IClientRepository clientRepository, IValidator<string> stringValidator)
    {
        _currencyRepository = currencyRepository;
        _clientRepository = clientRepository;
        _stringValidator = stringValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.ClientId)
                .NotEmpty()
                .MustAsync(_clientRepository.ExistsAsync)
                .WithMessage(Constants.Validation.Client.DoesNotExist);

            RuleFor(x => x.CurrencyId)
                .NotEmpty()
                .MustAsync(_currencyRepository.ExistsAsync)
                .WithMessage(Constants.Validation.Currency.DoesNotExist);

            RuleFor(x => x.Name)
                .SetValidator(_stringValidator, Constants.Validation.RuleSet.Names);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(Constants.Validation.Item.DescriptionMaxLength)
                .Matches("^[A-Z]")
                .WithMessage(Constants.Validation.Item.DescriptionShouldStartWithCapital);

            RuleFor(x => x.Price)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage(Constants.Validation.Item.PriceMustBePositive);
    }
}
