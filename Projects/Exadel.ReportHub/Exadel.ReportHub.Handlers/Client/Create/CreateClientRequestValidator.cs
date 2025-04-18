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
                    .MustAsync(NameMustNotExistsAsync)
                    .WithMessage(Constants.Validation.Client.NameTakenMessage);
            });
    }

    private async Task<bool> NameMustNotExistsAsync(string name, CancellationToken cancellationToken)
    {
        var nameExists = await _clientRepository.NameExistsAsync(name, cancellationToken);
        return !nameExists;
    }
}
