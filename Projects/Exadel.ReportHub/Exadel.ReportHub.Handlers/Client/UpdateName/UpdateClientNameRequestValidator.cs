using Exadel.ReportHub.Handlers.Validators;
using Exadel.ReportHub.RA;
using Exadel.ReportHub.RA.Abstract;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Client.UpdateName;

public class UpdateClientNameRequestValidator : AbstractValidator<UpdateClientNameRequest>
{
    private readonly IClientRepository _clientRepository;

    public UpdateClientNameRequestValidator(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .SetValidator(new ClientNameValidator(_clientRepository));
    }
}
