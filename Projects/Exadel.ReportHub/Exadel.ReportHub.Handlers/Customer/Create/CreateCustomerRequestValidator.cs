using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Customer;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Customer.Create;

public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    private readonly IValidator<CreateCustomerDTO> _createCustomerValidator;

    public CreateCustomerRequestValidator(IValidator<CreateCustomerDTO> createCustomerValidator)
    {
        _createCustomerValidator = createCustomerValidator;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.CreateCustomerDTO)
            .SetValidator(_createCustomerValidator);
    }
}
