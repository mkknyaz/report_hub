using Exadel.ReportHub.SDK.DTOs.Client;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Client.Create;

public class CreateClientRequestValidator : AbstractValidator<CreateClientRequest>
{
    private readonly IValidator<CreateClientDTO> _createClientValidator;

    public CreateClientRequestValidator(IValidator<CreateClientDTO> createClientValidator)
    {
        _createClientValidator = createClientValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleFor(x => x.CreateClientDto)
            .NotEmpty()
            .SetValidator(_createClientValidator);
    }
}
