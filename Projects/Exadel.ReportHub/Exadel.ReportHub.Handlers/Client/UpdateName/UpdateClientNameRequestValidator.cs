using Exadel.ReportHub.RA.Abstract;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Client.UpdateName;

public class UpdateClientNameRequestValidator : AbstractValidator<UpdateClientNameRequest>
{
    private readonly IClientRepository _clientRepository;
    private readonly IValidator<string> _stringValidator;

    public UpdateClientNameRequestValidator(IClientRepository clientRepository, IValidator<string> stringValidator)
    {
        _clientRepository = clientRepository;
        _stringValidator = stringValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.Name)
            .SetValidator(_stringValidator, Constants.Validation.RuleSet.Names)
            .MustAsync(async (name, cancellationToken) => !await _clientRepository.NameExistsAsync(name, cancellationToken))
            .WithMessage(Constants.Validation.Name.IsTaken);
    }
}
