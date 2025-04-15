using Exadel.ReportHub.Handlers.Validators;
using Exadel.ReportHub.RA.Abstract;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Client.Create;

public class CreateClientRequestValidator : AbstractValidator<CreateClientRequest>
{
    private readonly IClientRepository _clientRepository;

    public CreateClientRequestValidator(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.CreateClientDto)
            .NotEmpty()
            .ChildRules(child =>
            {
                child.RuleFor(x => x.Name)
                .NotEmpty()
                .SetValidator(new ClientNameValidator(_clientRepository));
            });
    }
}
