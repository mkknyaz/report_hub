using Exadel.ReportHub.RA.Abstract;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;

public class ClientNameValidator : AbstractValidator<string>
{
    private readonly IClientRepository _clientRepository;

    public ClientNameValidator(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(name => name)
            .Matches("^[A-Z]")
            .WithMessage(Constants.Validation.Client.ShouldStartWithCapitalMessage)
            .MaximumLength(Constants.Validation.Client.ClientMaximumNameLength)
            .MustAsync(NameMustNotExistsAsync)
            .WithMessage(Constants.Validation.Client.NameTakenMessage);
    }

    private async Task<bool> NameMustNotExistsAsync(string name, CancellationToken cancellationToken)
    {
        var nameExists = await _clientRepository.NameExistsAsync(name, cancellationToken);
        return !nameExists;
    }
}
